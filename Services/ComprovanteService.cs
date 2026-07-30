using ComprovantesApp.Data;
using ComprovantesApp.Models;
using ComprovantesApp.Models.Enums;
using ComprovantesApp.Models.Filtros;
using ComprovantesApp.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ComprovantesApp.Services
{
    public class ComprovanteService : IComprovanteService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ComprovanteService> _logger;

        public ComprovanteService(AppDbContext context, ILogger<ComprovanteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Comprovante>> ListarAsync(ComprovanteFiltro filtro)
        {
            var query = _context.Comprovantes
                .Include(c => c.Fornecedor)
                .AsQueryable();

            if (filtro.FornecedorId.HasValue)
                query = query.Where(c => c.FornecedorId == filtro.FornecedorId.Value);

            if (!string.IsNullOrWhiteSpace(filtro.NumeroDocumento))
                query = query.Where(c => c.NumeroDocumento.Contains(filtro.NumeroDocumento));

            if (filtro.Status.HasValue)
                query = query.Where(c => c.Status == filtro.Status.Value);

            if (filtro.EmissaoDe.HasValue)
                query = query.Where(c => c.DataEmissao >= filtro.EmissaoDe.Value);

            if (filtro.EmissaoAte.HasValue)
                query = query.Where(c => c.DataEmissao <= filtro.EmissaoAte.Value);

            return await query
                .OrderByDescending(c => c.DataCadastro)
                .ToListAsync();
        }

        public async Task<Comprovante?> ObterPorIdAsync(int id)
        {
            return await _context.Comprovantes
                .Include(c => c.Fornecedor)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<HistoricoComprovante>> ObterHistoricoAsync(int comprovanteId)
        {
            return await _context.Historicos
                .Where(h => h.ComprovanteId == comprovanteId)
                .OrderByDescending(h => h.DataHora)
                .ToListAsync();
        }

        public async Task CriarAsync(Comprovante comprovante)
        {
            await ValidarDadosBasicosAsync(comprovante);

            comprovante.DataCadastro = DateTime.Now;
            comprovante.Status = StatusComprovante.Recebido;

            _context.Comprovantes.Add(comprovante);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Comprovante {ComprovanteId} ({NumeroDocumento}) cadastrado para o fornecedor {FornecedorId}",
                comprovante.Id, comprovante.NumeroDocumento, comprovante.FornecedorId);

            await RegistrarHistoricoAsync(comprovante.Id, "Cadastro", "Comprovante cadastrado.");
        }

        public async Task AtualizarAsync(Comprovante comprovante)
        {
            var existente = await _context.Comprovantes.FindAsync(comprovante.Id)
                ?? throw new RegraDeNegocioException("Comprovante não encontrado.");

            if (existente.Status == StatusComprovante.IntegradoAoErp)
            {
                _logger.LogWarning("Tentativa de editar o comprovante {ComprovanteId} já integrado ao ERP", comprovante.Id);
                throw new RegraDeNegocioException("Este comprovante já foi integrado ao ERP e não pode mais ser editado.");
            }

            await ValidarDadosBasicosAsync(comprovante);

            existente.NumeroDocumento = comprovante.NumeroDocumento;
            existente.FornecedorId = comprovante.FornecedorId;
            existente.DataEmissao = comprovante.DataEmissao;
            existente.DataVencimento = comprovante.DataVencimento;
            existente.Valor = comprovante.Valor;
            existente.Descricao = comprovante.Descricao;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Comprovante {ComprovanteId} atualizado", existente.Id);

            await RegistrarHistoricoAsync(existente.Id, "Edição", "Dados do comprovante atualizados.");
        }

        public async Task ExcluirAsync(int id)
        {
            var comprovante = await _context.Comprovantes.FindAsync(id)
                ?? throw new RegraDeNegocioException("Comprovante não encontrado.");

            if (comprovante.Status != StatusComprovante.Recebido)
            {
                _logger.LogWarning("Tentativa de excluir o comprovante {ComprovanteId} com status {Status}", id, comprovante.Status);
                throw new RegraDeNegocioException("Só é possível excluir comprovantes com status \"Recebido\".");
            }

            _context.Comprovantes.Remove(comprovante);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Comprovante {ComprovanteId} excluído", id);
        }

        public async Task ValidarAsync(int id)
        {
            var comprovante = await _context.Comprovantes.FindAsync(id)
                ?? throw new RegraDeNegocioException("Comprovante não encontrado.");

            if (comprovante.Status == StatusComprovante.IntegradoAoErp)
                throw new RegraDeNegocioException("Este comprovante já foi integrado ao ERP e não pode mais ser alterado.");

            if (comprovante.Valor <= 0)
            {
                _logger.LogWarning("Tentativa de validar o comprovante {ComprovanteId} com valor inválido ({Valor})", id, comprovante.Valor);
                throw new RegraDeNegocioException("Não é possível validar um comprovante com valor menor ou igual a zero.");
            }

            comprovante.Status = StatusComprovante.Validado;
            comprovante.DataValidacao = DateTime.Now;
            comprovante.ObservacaoInconsistencia = null;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Comprovante {ComprovanteId} validado", id);

            await RegistrarHistoricoAsync(comprovante.Id, "Validação", "Comprovante marcado como Validado.");
        }

        public async Task MarcarInconsistenciaAsync(int id, string motivo)
        {
            var comprovante = await _context.Comprovantes.FindAsync(id)
                ?? throw new RegraDeNegocioException("Comprovante não encontrado.");

            if (comprovante.Status == StatusComprovante.IntegradoAoErp)
                throw new RegraDeNegocioException("Este comprovante já foi integrado ao ERP e não pode mais ser alterado.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new RegraDeNegocioException("Informe o motivo da inconsistência.");

            comprovante.Status = StatusComprovante.ComInconsistencia;
            comprovante.ObservacaoInconsistencia = motivo;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Inconsistência registrada no comprovante {ComprovanteId}: {Motivo}", id, motivo);

            await RegistrarHistoricoAsync(comprovante.Id, "Inconsistência registrada", motivo);
        }

        public async Task CancelarAsync(int id, string motivo)
        {
            var comprovante = await _context.Comprovantes.FindAsync(id)
                ?? throw new RegraDeNegocioException("Comprovante não encontrado.");

            if (comprovante.Status == StatusComprovante.IntegradoAoErp)
                throw new RegraDeNegocioException("Este comprovante já foi integrado ao ERP e não pode mais ser alterado.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new RegraDeNegocioException("Informe o motivo do cancelamento.");

            comprovante.Status = StatusComprovante.Cancelado;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Comprovante {ComprovanteId} cancelado: {Motivo}", id, motivo);

            await RegistrarHistoricoAsync(comprovante.Id, "Cancelamento", motivo);
        }

        public async Task IntegrarAsync(int id)
        {
            var comprovante = await _context.Comprovantes.FindAsync(id)
                ?? throw new RegraDeNegocioException("Comprovante não encontrado.");

            if (comprovante.Status != StatusComprovante.Validado)
            {
                _logger.LogWarning("Tentativa de integrar o comprovante {ComprovanteId} com status {Status}", id, comprovante.Status);
                throw new RegraDeNegocioException("Só é possível integrar comprovantes com status \"Validado\".");
            }

            comprovante.Status = StatusComprovante.IntegradoAoErp;
            comprovante.DataIntegracao = DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Comprovante {ComprovanteId} integrado ao ERP", id);

            await RegistrarHistoricoAsync(comprovante.Id, "Integração ao ERP", "Comprovante integrado ao ERP (simulado).");
        }

        private async Task ValidarDadosBasicosAsync(Comprovante comprovante)
        {
            if (string.IsNullOrWhiteSpace(comprovante.NumeroDocumento))
                throw new RegraDeNegocioException("O número do documento é obrigatório.");

            if (comprovante.NumeroDocumento.Length > 30)
                throw new RegraDeNegocioException("O número do documento deve ter no máximo 30 caracteres.");

            if (comprovante.Valor <= 0)
                throw new RegraDeNegocioException("O valor do comprovante deve ser maior que zero.");

            if (comprovante.DataVencimento < comprovante.DataEmissao)
                throw new RegraDeNegocioException("A data de vencimento não pode ser anterior à data de emissão.");

            var fornecedorExiste = await _context.Fornecedores.AnyAsync(f => f.Id == comprovante.FornecedorId);
            if (!fornecedorExiste)
                throw new RegraDeNegocioException("Fornecedor inválido.");

            var duplicado = await _context.Comprovantes.AnyAsync(c =>
                c.FornecedorId == comprovante.FornecedorId &&
                c.NumeroDocumento == comprovante.NumeroDocumento &&
                c.Id != comprovante.Id);

            if (duplicado)
                throw new RegraDeNegocioException("Este fornecedor já possui um comprovante cadastrado com esse número de documento.");
        }

        private async Task RegistrarHistoricoAsync(int comprovanteId, string acao, string? descricao)
        {
            _context.Historicos.Add(new HistoricoComprovante
            {
                ComprovanteId = comprovanteId,
                Acao = acao,
                Descricao = descricao,
                DataHora = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }
    }
}

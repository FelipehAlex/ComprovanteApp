using ComprovantesApp.Data;
using ComprovantesApp.Models;
using ComprovantesApp.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ComprovantesApp.Services
{
    public class FornecedorService : IFornecedorService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FornecedorService> _logger;

        public FornecedorService(AppDbContext context, ILogger<FornecedorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Fornecedor>> ListarAsync()
        {
            return await _context.Fornecedores
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }

        public async Task<Fornecedor?> ObterPorIdAsync(int id)
        {
            return await _context.Fornecedores.FindAsync(id);
        }

        public async Task CriarAsync(Fornecedor fornecedor)
        {
            await ValidarNomeECnpjAsync(fornecedor);
            _context.Fornecedores.Add(fornecedor);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Fornecedor {FornecedorId} ({Nome}) cadastrado", fornecedor.Id, fornecedor.Nome);
        }

        public async Task AtualizarAsync(Fornecedor fornecedor)
        {
            await ValidarNomeECnpjAsync(fornecedor);
            _context.Fornecedores.Update(fornecedor);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Fornecedor {FornecedorId} atualizado", fornecedor.Id);
        }

        public async Task InativarAsync(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id)
                ?? throw new RegraDeNegocioException("Fornecedor não encontrado.");

            fornecedor.Ativo = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Fornecedor {FornecedorId} inativado", id);
        }

        public async Task AtivarAsync(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id)
                ?? throw new RegraDeNegocioException("Fornecedor não encontrado.");

            fornecedor.Ativo = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Fornecedor {FornecedorId} ativado", id);
        }

        private async Task ValidarNomeECnpjAsync(Fornecedor fornecedor)
        {
            if (string.IsNullOrWhiteSpace(fornecedor.Nome))
                throw new RegraDeNegocioException("O nome do fornecedor é obrigatório.");

            if (string.IsNullOrWhiteSpace(fornecedor.Cnpj))
                throw new RegraDeNegocioException("O CNPJ do fornecedor é obrigatório.");

            var cnpjJaExiste = await _context.Fornecedores
                .AnyAsync(f => f.Cnpj == fornecedor.Cnpj && f.Id != fornecedor.Id);

            if (cnpjJaExiste)
            {
                _logger.LogWarning("Tentativa de cadastro com CNPJ {Cnpj} já existente", fornecedor.Cnpj);
                throw new RegraDeNegocioException("Já existe um fornecedor cadastrado com esse CNPJ.");
            }
        }
    }
}

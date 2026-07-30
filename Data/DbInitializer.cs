using ComprovantesApp.Models;
using ComprovantesApp.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ComprovantesApp.Data
{
    /// <summary>
    /// Popula o banco com dados de exemplo na primeira execução, depois que as migrations
    /// já criaram as tabelas. Não faz nada se já existir algum fornecedor cadastrado.
    /// </summary>
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Fornecedores.AnyAsync())
                return;

            var hotelIpanema = new Fornecedor { Nome = "Hotel Ipanema Palace", Cnpj = "12.345.678/0001-90", TipoFornecedor = TipoFornecedor.Hotel, Ativo = true };
            var hotelSerraVerde = new Fornecedor { Nome = "Hotel Serra Verde", Cnpj = "23.456.789/0001-01", TipoFornecedor = TipoFornecedor.Hotel, Ativo = true };
            var transportadora = new Fornecedor { Nome = "Transportadora Rota Certa", Cnpj = "34.567.890/0001-12", TipoFornecedor = TipoFornecedor.Fornecedor, Ativo = true };
            var locadora = new Fornecedor { Nome = "Locadora VeloCar", Cnpj = "45.678.901/0001-23", TipoFornecedor = TipoFornecedor.Fornecedor, Ativo = false };

            context.Fornecedores.AddRange(hotelIpanema, hotelSerraVerde, transportadora, locadora);
            await context.SaveChangesAsync();

            var agora = DateTime.Now;

            var recebido = new Comprovante
            {
                NumeroDocumento = "NF-1001",
                FornecedorId = hotelIpanema.Id,
                DataEmissao = agora.AddDays(-10),
                DataVencimento = agora.AddDays(5),
                Valor = 1580.50m,
                Descricao = "Hospedagem - evento corporativo",
                Status = StatusComprovante.Recebido,
                DataCadastro = agora.AddDays(-10)
            };

            var validado = new Comprovante
            {
                NumeroDocumento = "NF-2002",
                FornecedorId = hotelSerraVerde.Id,
                DataEmissao = agora.AddDays(-20),
                DataVencimento = agora.AddDays(-5),
                Valor = 940.00m,
                Descricao = "Diárias - visita técnica",
                Status = StatusComprovante.Validado,
                DataCadastro = agora.AddDays(-20),
                DataValidacao = agora.AddDays(-18)
            };

            var integrado = new Comprovante
            {
                NumeroDocumento = "NF-3003",
                FornecedorId = transportadora.Id,
                DataEmissao = agora.AddDays(-30),
                DataVencimento = agora.AddDays(-15),
                Valor = 320.75m,
                Descricao = "Transporte de equipe",
                Status = StatusComprovante.IntegradoAoErp,
                DataCadastro = agora.AddDays(-30),
                DataValidacao = agora.AddDays(-28),
                DataIntegracao = agora.AddDays(-25)
            };

            var comInconsistencia = new Comprovante
            {
                NumeroDocumento = "NF-4004",
                FornecedorId = hotelIpanema.Id,
                DataEmissao = agora.AddDays(-3),
                DataVencimento = agora.AddDays(10),
                Valor = 210.00m,
                Descricao = "Ajuste de diária - valor divergente do contrato",
                Status = StatusComprovante.ComInconsistencia,
                DataCadastro = agora.AddDays(-3),
                ObservacaoInconsistencia = "Valor cobrado divergente do contratado com o hotel."
            };

            context.Comprovantes.AddRange(recebido, validado, integrado, comInconsistencia);
            await context.SaveChangesAsync();

            context.Historicos.AddRange(
                new HistoricoComprovante { ComprovanteId = recebido.Id, DataHora = agora.AddDays(-10), Acao = "Cadastro", Descricao = "Comprovante cadastrado." },

                new HistoricoComprovante { ComprovanteId = validado.Id, DataHora = agora.AddDays(-20), Acao = "Cadastro", Descricao = "Comprovante cadastrado." },
                new HistoricoComprovante { ComprovanteId = validado.Id, DataHora = agora.AddDays(-18), Acao = "Validação", Descricao = "Comprovante validado." },

                new HistoricoComprovante { ComprovanteId = integrado.Id, DataHora = agora.AddDays(-30), Acao = "Cadastro", Descricao = "Comprovante cadastrado." },
                new HistoricoComprovante { ComprovanteId = integrado.Id, DataHora = agora.AddDays(-28), Acao = "Validação", Descricao = "Comprovante validado." },
                new HistoricoComprovante { ComprovanteId = integrado.Id, DataHora = agora.AddDays(-25), Acao = "Integração ao ERP", Descricao = "Comprovante integrado ao ERP (simulado)." },

                new HistoricoComprovante { ComprovanteId = comInconsistencia.Id, DataHora = agora.AddDays(-3), Acao = "Cadastro", Descricao = "Comprovante cadastrado." },
                new HistoricoComprovante { ComprovanteId = comInconsistencia.Id, DataHora = agora.AddDays(-2), Acao = "Inconsistência registrada", Descricao = "Valor cobrado divergente do contratado com o hotel." }
            );

            await context.SaveChangesAsync();
        }
    }
}

using ComprovantesApp.Models;

namespace ComprovantesApp.Services
{
    public interface IFornecedorService
    {
        Task<List<Fornecedor>> ListarAsync();
        Task<Fornecedor?> ObterPorIdAsync(int id);
        Task CriarAsync(Fornecedor fornecedor);
        Task AtualizarAsync(Fornecedor fornecedor);
        Task InativarAsync(int id);
        Task AtivarAsync(int id);
    }
}

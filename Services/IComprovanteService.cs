using ComprovantesApp.Models;
using ComprovantesApp.Models.Filtros;

namespace ComprovantesApp.Services
{
    public interface IComprovanteService
    {
        Task<List<Comprovante>> ListarAsync(ComprovanteFiltro filtro);
        Task<Comprovante?> ObterPorIdAsync(int id);
        Task<List<HistoricoComprovante>> ObterHistoricoAsync(int comprovanteId);

        Task CriarAsync(Comprovante comprovante);
        Task AtualizarAsync(Comprovante comprovante);
        Task ExcluirAsync(int id);

        Task ValidarAsync(int id);
        Task MarcarInconsistenciaAsync(int id, string motivo);
        Task IntegrarAsync(int id);
        Task CancelarAsync(int id, string motivo);
    }
}

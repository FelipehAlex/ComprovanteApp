using ComprovantesApp.Models;
using ComprovantesApp.Models.Enums;
using ComprovantesApp.Models.Filtros;
using ComprovantesApp.Services;
using ComprovantesApp.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ComprovantesApp.Pages.Comprovantes
{
    public class IndexModel : PageModel
    {
        private readonly IComprovanteService _comprovanteService;
        private readonly IFornecedorService _fornecedorService;

        public IndexModel(IComprovanteService comprovanteService, IFornecedorService fornecedorService)
        {
            _comprovanteService = comprovanteService;
            _fornecedorService = fornecedorService;
        }

        [BindProperty(SupportsGet = true)]
        public ComprovanteFiltro Filtro { get; set; } = new();

        public List<Comprovante> Comprovantes { get; set; } = new();
        public List<Fornecedor> Fornecedores { get; set; } = new();

        public async Task OnGetAsync()
        {
            Fornecedores = await _fornecedorService.ListarAsync();
            Comprovantes = await _comprovanteService.ListarAsync(Filtro);
        }

        public async Task<IActionResult> OnPostValidarAsync(int id)
        {
            try
            {
                await _comprovanteService.ValidarAsync(id);
                TempData["Sucesso"] = "Comprovante validado com sucesso.";
            }
            catch (RegraDeNegocioException ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToPage(new
            {
                Filtro.FornecedorId,
                Filtro.NumeroDocumento,
                Filtro.Status,
                Filtro.EmissaoDe,
                Filtro.EmissaoAte
            });
        }

        public async Task<IActionResult> OnPostIntegrarAsync(int id)
        {
            try
            {
                await _comprovanteService.IntegrarAsync(id);
                TempData["Sucesso"] = "Comprovante integrado ao ERP com sucesso.";
            }
            catch (RegraDeNegocioException ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToPage(new
            {
                Filtro.FornecedorId,
                Filtro.NumeroDocumento,
                Filtro.Status,
                Filtro.EmissaoDe,
                Filtro.EmissaoAte
            });
        }

        public async Task<IActionResult> OnPostExcluirAsync(int id)
        {
            try
            {
                await _comprovanteService.ExcluirAsync(id);
                TempData["Sucesso"] = "Comprovante excluído com sucesso.";
            }
            catch (RegraDeNegocioException ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToPage(new
            {
                Filtro.FornecedorId,
                Filtro.NumeroDocumento,
                Filtro.Status,
                Filtro.EmissaoDe,
                Filtro.EmissaoAte
            });
        }
        public async Task<IActionResult> OnPostCancelarAsync(int id, string motivo)
        {
            try
            {
                await _comprovanteService.CancelarAsync(id, motivo);
                TempData["Sucesso"] = "Comprovante cancelado com sucesso.";
            }
            catch (RegraDeNegocioException ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToPage(new
            {
                Filtro.FornecedorId,
                Filtro.NumeroDocumento,
                Filtro.Status,
                Filtro.EmissaoDe,
                Filtro.EmissaoAte
            });
        }
    }
}

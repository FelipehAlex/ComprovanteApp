using ComprovantesApp.Models;
using ComprovantesApp.Services;
using ComprovantesApp.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ComprovantesApp.Pages.Fornecedores
{
    public class IndexModel : PageModel
    {
        private readonly IFornecedorService _fornecedorService;

        public IndexModel(IFornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
        }

        public List<Fornecedor> Fornecedores { get; set; } = new();

        public async Task OnGetAsync()
        {
            Fornecedores = await _fornecedorService.ListarAsync();
        }

        public async Task<IActionResult> OnPostInativarAsync(int id)
        {
            try
            {
                await _fornecedorService.InativarAsync(id);
                TempData["Sucesso"] = "Fornecedor inativado com sucesso.";
            }
            catch (RegraDeNegocioException ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAtivarAsync(int id)
        {
            try
            {
                await _fornecedorService.AtivarAsync(id);
                TempData["Sucesso"] = "Fornecedor ativado com sucesso.";
            }
            catch (RegraDeNegocioException ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToPage();
        }
    }
}

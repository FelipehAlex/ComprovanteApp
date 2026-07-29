using ComprovantesApp.Models;
using ComprovantesApp.Models.Enums;
using ComprovantesApp.Services;
using ComprovantesApp.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ComprovantesApp.Pages.Fornecedores
{
    public class EditarModel : PageModel
    {
        private readonly IFornecedorService _fornecedorService;

        public EditarModel(IFornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
        }

        [BindProperty]
        public Fornecedor Fornecedor { get; set; } = new();

        public bool EhEdicao { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var fornecedor = await _fornecedorService.ObterPorIdAsync(id.Value);
                if (fornecedor is null)
                    return NotFound();

                Fornecedor = fornecedor;
                EhEdicao = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            EhEdicao = Fornecedor.Id != 0;

            if (!ModelState.IsValid)
                return Page();

            try
            {
                if (EhEdicao)
                {
                    await _fornecedorService.AtualizarAsync(Fornecedor);
                    TempData["Sucesso"] = "Fornecedor atualizado com sucesso.";
                }
                else
                {
                    await _fornecedorService.CriarAsync(Fornecedor);
                    TempData["Sucesso"] = "Fornecedor cadastrado com sucesso.";
                }
            }
            catch (RegraDeNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}

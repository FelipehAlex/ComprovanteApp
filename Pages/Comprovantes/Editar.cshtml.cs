using ComprovantesApp.Models;
using ComprovantesApp.Services;
using ComprovantesApp.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ComprovantesApp.Pages.Comprovantes
{
    public class EditarModel : PageModel
    {
        private readonly IComprovanteService _comprovanteService;
        private readonly IFornecedorService _fornecedorService;

        public EditarModel(IComprovanteService comprovanteService, IFornecedorService fornecedorService)
        {
            _comprovanteService = comprovanteService;
            _fornecedorService = fornecedorService;
        }

        [BindProperty]
        public Comprovante Comprovante { get; set; } = new()
        {
            DataEmissao = DateTime.Today,
            DataVencimento = DateTime.Today
        };

        public List<Fornecedor> Fornecedores { get; set; } = new();
        public bool EhEdicao { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Fornecedores = await _fornecedorService.ListarAsync();

            if (id.HasValue)
            {
                var comprovante = await _comprovanteService.ObterPorIdAsync(id.Value);
                if (comprovante is null)
                    return NotFound();

                if (comprovante.Status == ComprovantesApp.Models.Enums.StatusComprovante.IntegradoAoErp)
                {
                    TempData["Erro"] = "Este comprovante já foi integrado ao ERP e não pode mais ser editado.";
                    return RedirectToPage("Index");
                }

                Comprovante = comprovante;
                EhEdicao = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            EhEdicao = Comprovante.Id != 0;
            Fornecedores = await _fornecedorService.ListarAsync();

            if (!ModelState.IsValid)
                return Page();

            try
            {
                if (EhEdicao)
                {
                    await _comprovanteService.AtualizarAsync(Comprovante);
                    TempData["Sucesso"] = "Comprovante atualizado com sucesso.";
                }
                else
                {
                    await _comprovanteService.CriarAsync(Comprovante);
                    TempData["Sucesso"] = "Comprovante cadastrado com sucesso.";
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

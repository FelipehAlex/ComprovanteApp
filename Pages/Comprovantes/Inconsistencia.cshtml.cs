using ComprovantesApp.Models;
using ComprovantesApp.Services;
using ComprovantesApp.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ComprovantesApp.Pages.Comprovantes
{
    public class InconsistenciaModel : PageModel
    {
        private readonly IComprovanteService _comprovanteService;

        public InconsistenciaModel(IComprovanteService comprovanteService)
        {
            _comprovanteService = comprovanteService;
        }

        public Comprovante? Comprovante { get; set; }

        [BindProperty]
        public int ComprovanteId { get; set; }

        [BindProperty]
        public string Motivo { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Comprovante = await _comprovanteService.ObterPorIdAsync(id);
            if (Comprovante is null)
                return NotFound();

            ComprovanteId = id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _comprovanteService.MarcarInconsistenciaAsync(ComprovanteId, Motivo);
                TempData["Sucesso"] = "Inconsistência registrada com sucesso.";
                return RedirectToPage("Index");
            }
            catch (RegraDeNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Comprovante = await _comprovanteService.ObterPorIdAsync(ComprovanteId);
                return Page();
            }
        }
    }
}

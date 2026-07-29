using ComprovantesApp.Models;
using ComprovantesApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ComprovantesApp.Pages.Comprovantes
{
    public class HistoricoModel : PageModel
    {
        private readonly IComprovanteService _comprovanteService;

        public HistoricoModel(IComprovanteService comprovanteService)
        {
            _comprovanteService = comprovanteService;
        }

        public Comprovante? Comprovante { get; set; }
        public List<HistoricoComprovante> Historicos { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Comprovante = await _comprovanteService.ObterPorIdAsync(id);
            if (Comprovante is null)
                return NotFound();

            Historicos = await _comprovanteService.ObterHistoricoAsync(id);
            return Page();
        }
    }
}

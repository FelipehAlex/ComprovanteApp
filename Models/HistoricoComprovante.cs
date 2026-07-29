using System.ComponentModel.DataAnnotations;

namespace ComprovantesApp.Models
{
    public class HistoricoComprovante
    {
        public int Id { get; set; }

        public int ComprovanteId { get; set; }
        public Comprovante? Comprovante { get; set; }

        [Display(Name = "Data/Hora")]
        public DateTime DataHora { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        [Display(Name = "Ação")]
        public string Acao { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }
    }
}

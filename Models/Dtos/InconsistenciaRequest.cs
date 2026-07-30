using System.ComponentModel.DataAnnotations;

namespace ComprovantesApp.Models.Dtos
{
    public class InconsistenciaRequest
    {
        [Required(ErrorMessage = "Informe o motivo da inconsistência.")]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;
    }
}

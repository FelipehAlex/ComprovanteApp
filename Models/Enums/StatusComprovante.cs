using System.ComponentModel.DataAnnotations;

namespace ComprovantesApp.Models.Enums
{
    public enum StatusComprovante
    {
        [Display(Name = "Recebido")]
        Recebido = 1,

        [Display(Name = "Em validação")]
        EmValidacao = 2,

        [Display(Name = "Validado")]
        Validado = 3,

        [Display(Name = "inconsistência")]
        ComInconsistencia = 4,

        [Display(Name = "Integrado ao ERP")]
        IntegradoAoErp = 5,
    }
}

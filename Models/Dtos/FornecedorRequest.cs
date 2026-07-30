using System.ComponentModel.DataAnnotations;
using ComprovantesApp.Models.Enums;

namespace ComprovantesApp.Models.Dtos
{
    public class FornecedorRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [StringLength(18)]
        public string Cnpj { get; set; } = string.Empty;

        [Required]
        public TipoFornecedor TipoFornecedor { get; set; }

        public bool Ativo { get; set; } = true;
    }
}

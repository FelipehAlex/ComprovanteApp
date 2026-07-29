using System.ComponentModel.DataAnnotations;
using ComprovantesApp.Models.Enums;

namespace ComprovantesApp.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [StringLength(18)]
        [Display(Name = "CNPJ")]
        public string Cnpj { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tipo")]
        public TipoFornecedor TipoFornecedor { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        public ICollection<Comprovante> Comprovantes { get; set; } = new List<Comprovante>();
    }
}

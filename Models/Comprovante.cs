using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ComprovantesApp.Models.Enums;

namespace ComprovantesApp.Models
{
    public class Comprovante
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O número do documento é obrigatório.")]
        [StringLength(30, ErrorMessage = "O número do documento deve ter no máximo 30 caracteres.")]
        [Display(Name = "Número do documento")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione um fornecedor.")]
        [Display(Name = "Fornecedor")]
        public int FornecedorId { get; set; }
        public Fornecedor? Fornecedor { get; set; }

        [Required(ErrorMessage = "A data de emissão é obrigatória.")]
        [Display(Name = "Data de emissão")]
        [DataType(DataType.Date)]
        public DateTime DataEmissao { get; set; }

        [Required(ErrorMessage = "A data de vencimento é obrigatória.")]
        [Display(Name = "Data de vencimento")]
        [DataType(DataType.Date)]
        public DateTime DataVencimento { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Valor")]
        public decimal Valor { get; set; }

        [StringLength(500)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Required]
        [Display(Name = "Status")]
        public StatusComprovante Status { get; set; } = StatusComprovante.Recebido;

        [Display(Name = "Data de cadastro")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Data de validação")]
        public DateTime? DataValidacao { get; set; }

        [Display(Name = "Data de integração")]
        public DateTime? DataIntegracao { get; set; }

        [StringLength(500)]
        [Display(Name = "Observação de inconsistência")]
        public string? ObservacaoInconsistencia { get; set; }

        public ICollection<HistoricoComprovante> Historicos { get; set; } = new List<HistoricoComprovante>();
    }
}

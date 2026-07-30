using System.ComponentModel.DataAnnotations;

namespace ComprovantesApp.Models.Dtos
{
    public class ComprovanteRequest
    {
        [Required(ErrorMessage = "O número do documento é obrigatório.")]
        [StringLength(30, ErrorMessage = "O número do documento deve ter no máximo 30 caracteres.")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione um fornecedor.")]
        public int FornecedorId { get; set; }

        [Required(ErrorMessage = "A data de emissão é obrigatória.")]
        public DateTime DataEmissao { get; set; }

        [Required(ErrorMessage = "A data de vencimento é obrigatória.")]
        public DateTime DataVencimento { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        public decimal Valor { get; set; }

        [StringLength(500)]
        public string? Descricao { get; set; }
    }
}

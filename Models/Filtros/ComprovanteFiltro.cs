using ComprovantesApp.Models.Enums;

namespace ComprovantesApp.Models.Filtros
{
    public class ComprovanteFiltro
    {
        public int? FornecedorId { get; set; }
        public string? NumeroDocumento { get; set; }
        public StatusComprovante? Status { get; set; }
        public DateTime? EmissaoDe { get; set; }
        public DateTime? EmissaoAte { get; set; }
    }
}

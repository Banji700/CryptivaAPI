using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptiva.Dtos
{
    public class PortfolioDto
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal CashBalance { get; set; }
    
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalBalance { get; set; }
    }
}

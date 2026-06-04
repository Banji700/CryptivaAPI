using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptiva.Models
{
    public class PortfolioSnapshot
    {
        public int Id { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int PortfolioId { get; set; }
        public PortfolioModel Portfolio { get; set; } = null!;
    }
}

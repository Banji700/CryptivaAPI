using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptiva.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public TransactionType Type { get; set; }

        public string? CoinId { get; set; }
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int PortfolioId { get; set; }
        public PortfolioModel Portfolio { get; set; } = null!;
    }
}

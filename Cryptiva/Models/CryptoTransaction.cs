using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptiva.Models
{
    public class CryptoTransaction
    {
        public int Id { get; set; }

        public string CoinId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [Column(TypeName ="decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtTrade { get; set; }

        public string Type { get; set; } = string.Empty; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int PortfolioId { get; set; }
    }
}

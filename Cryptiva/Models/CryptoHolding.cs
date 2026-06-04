using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptiva.Models
{
    public class CryptoHolding
    {
        public int Id { get; set; }
        public string CoinId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal AverageBuyPrice { get; set; }

        public int PortfolioId { get; set; }



    }
}

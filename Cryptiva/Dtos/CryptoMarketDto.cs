using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cryptiva.Dtos
{
    public class CryptoMarketDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
       // [Column(TypeName = "decimal(18,2)")]
       // [JsonPropertyName("current_price")]
        public decimal? CurrentPrice { get; set; }
       // [Column(TypeName = "decimal(18,2)")]

       // [JsonPropertyName("market_cap")]
        public decimal? MarketCap { get; set; }
        
       // [JsonPropertyName("price_change_percentage_24h")]
        public decimal? PriceChangePercentage24h { get; set; }

    }
}

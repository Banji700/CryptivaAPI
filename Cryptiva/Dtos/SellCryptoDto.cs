namespace Cryptiva.Dtos
{
    public class SellCryptoDto
    {
        public string CoinId { get; set; } = string.Empty;
        public decimal QuantityToSell { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}

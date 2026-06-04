namespace Cryptiva.Dtos
{
    public class BuyCryptoDto
    {
        public string CoinId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal AmountToSpend { get; set; }

    }
}

namespace Cryptiva.Dtos
{
    public class HoldingsDto
    {
        public string CoinId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public decimal Quantity { get; set; }
        public decimal AverageBuyPrice { get; set; }

    }
}

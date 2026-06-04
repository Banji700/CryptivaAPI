namespace Cryptiva.Models
{
    public class FavoriteCoin
    {
        public int Id { get; set; }
        public string CoinId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;

        public int PortfolioId { get; set; }
        public PortfolioModel Portfolio { get; set; } = null!;

    }
}

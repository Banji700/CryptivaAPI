namespace Cryptiva.Models
{
    public class WatchListItem
    {
        public int Id { get; set; }

        public string CoinId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string AppUserId { get; set; } = string.Empty;
    }
}

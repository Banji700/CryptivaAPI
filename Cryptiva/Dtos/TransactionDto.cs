namespace Cryptiva.Dtos
{
    public class TransactionDto
    {
        
        
            public string Type { get; set; } = string.Empty;

            public string? CoinId { get; set; }
            public string? Symbol { get; set; }
            public string? Name { get; set; }

            public decimal Amount { get; set; }
            public decimal? Quantity { get; set; }
            public decimal? Price { get; set; }

            public DateTime CreatedAt { get; set; }
        
    }
}

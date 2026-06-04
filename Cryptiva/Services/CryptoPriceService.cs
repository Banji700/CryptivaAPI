
namespace Cryptiva.Services
{
    public class CryptoPriceService
    {
        private readonly HttpClient _httpClient;

        public CryptoPriceService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "CryptivaApp/1.0"
            );
        }

        public async Task<Dictionary<string, decimal>> GetCurrentPricesAsync(List<string> coinIds)
        {
            if (coinIds.Count == 0)
                return [];

            var ids = string.Join(",", coinIds);

            var url =
                $"https://api.coingecko.com/api/v3/simple/price?ids={ids}&vs_currencies=gbp";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Dictionary<string, Dictionary<string, decimal>>>(url);

                if (response == null)
                    return [];

                return response.ToDictionary(
                    x => x.Key,
                    x => x.Value["gbp"]
                );
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"CoinGecko price fetch failed: {ex.Message}");
                return [];
            }
        }
    }
}

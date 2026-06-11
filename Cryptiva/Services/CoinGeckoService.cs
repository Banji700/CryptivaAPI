using Cryptiva.Dtos;
using System.Text.Json;

namespace Cryptiva.Services
{
    public class CoinGeckoService
    {
        readonly HttpClient _httpClient;

        public CoinGeckoService(HttpClient httpClient)
        {
            _httpClient= httpClient;

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Cryptiva/1.0"); 
        }

        private List<CryptoMarketDto>? _cachedMarkets;
        private DateTime _cacheExpiry = DateTime.MinValue;

        public async Task<List<CryptoMarketDto>?> GetMarketsAsync()
        {
            if (_cachedMarkets != null && DateTime.UtcNow < _cacheExpiry)
            {
                return _cachedMarkets;
            }

            var url = "https://api.coingecko.com/api/v3/coins/markets" +
          "?vs_currency=gbp" +
          "&order=market_cap_desc" +
          "&per_page=20" +
          "&page=1" +
          "&sparkline=false";

            try
            {
                var response = await _httpClient.GetStringAsync(url);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                var markets = JsonSerializer.Deserialize<List<CoinGeckoDto>>(response, options);

                var mappedMarkets = markets?.Select(x => new CryptoMarketDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Symbol = x.Symbol,
                    Image = x.Image,
                    CurrentPrice = x.CurrentPrice,
                    MarketCap = x.MarketCap,
                    PriceChangePercentage24h = x.PriceChangePercentage24h

                }).ToList();

                _cachedMarkets = mappedMarkets;
                _cacheExpiry = DateTime.UtcNow.AddMinutes(3);

                return mappedMarkets ?? [];
            }
            catch (HttpRequestException)
            {
                return _cachedMarkets ?? [];    
            }
        }
        
    }
}

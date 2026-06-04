using Cryptiva.Services;
using Cryptiva.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace Cryptiva.SignalR
{
    public class CryproBroadcastService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<CryptoHub> _hubContext;

        public CryproBroadcastService(IServiceScopeFactory scopeFactory, IHubContext<CryptoHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var coinGeckoService = scope.ServiceProvider.GetRequiredService<CoinGeckoService>();

                var markets = await coinGeckoService.GetMarketsAsync();

                await _hubContext.Clients.All.SendAsync(
               "CryptoPricesUpdated",
               markets,
               stoppingToken
           );

                await Task.Delay(15000, stoppingToken);
            }
        }
    }
}

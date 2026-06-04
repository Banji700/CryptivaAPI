using Cryptiva.Dtos;
using Cryptiva.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cryptiva.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoController : ControllerBase
    {
        private readonly CoinGeckoService _coinGeckoService;

        public CryptoController(CoinGeckoService coinGeckoService)
        {
            _coinGeckoService = coinGeckoService;
        }

        [HttpGet("markets")]
        public async Task<ActionResult<CryptoMarketDto>> GetMarkets()
        {
            var result = await _coinGeckoService.GetMarketsAsync();
            return Ok(result);
        }
    }
}

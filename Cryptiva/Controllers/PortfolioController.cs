using Cryptiva.Data;
using Cryptiva.Dtos;
using Cryptiva.Interfaces;
using Cryptiva.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cryptiva.Controllers
{
    [Authorize]
    
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;

        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;   
        }

        [HttpGet]
        public async Task<ActionResult<PortfolioDto>> GetPortfolio()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var portfolio = await _portfolioService.GetPortfolioAsync(userId);

            if (portfolio == null)
                return NotFound("Portfolio not found");

            return Ok(portfolio);
        }

        [HttpPost("deposit")]
        public async Task<ActionResult<PortfolioDto>> Deposit(DepositDto depositDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null) return Unauthorized();

            var portfolio = await _portfolioService.DepositAsync(userId, depositDto.Amount);

            if (portfolio == null) return BadRequest("Invalid Deposit");

            return Ok(portfolio);
        }


        [HttpPost("buy")]
        public async Task<ActionResult<PortfolioDto>> Buy(BuyCryptoDto buyCryptoDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null) return Unauthorized();

            try
            {
                var portfolio = await _portfolioService.BuyCryptoAsync(userId, buyCryptoDto);

                if(portfolio== null)
                    return BadRequest("Invalid Buy Request");

                return Ok(portfolio);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("holdings")]

        public async Task<ActionResult<IReadOnlyList<HoldingsDto>>> GetHoldings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null)
                return Unauthorized();

            var holdings = await _portfolioService.GetHoldingsAsync(userId);
            
            return Ok(holdings);
        }

        [HttpPost("sell")]
        public async Task<ActionResult<PortfolioDto>> Sell(SellCryptoDto sellCryptoDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            try
            {
                var portfolio = await _portfolioService.SellCryptoAsync(userId, sellCryptoDto);

                if (portfolio == null)
                    return BadRequest("Invalid sell request");

                return Ok(portfolio);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetTransactions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var transactions = await _portfolioService.GetTransactionsAsync(userId);

            return Ok(transactions);
        }

        [HttpGet("snapshots")]
        public async Task<ActionResult<IReadOnlyList<SnapshotDto>>> GetSnapshots()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var snapshots = await _portfolioService.GetSnapshotsAsync(userId);

            return Ok(snapshots);
        }

        [HttpGet("favorites")]
        public async Task<ActionResult<IReadOnlyList<FavoriteCoinDto>>> GetFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var favorites = await _portfolioService.GetFavoritesAsync(userId);

            return Ok(favorites);
        }

        [HttpPost("favorites")]
        public async Task<ActionResult<FavoriteCoinDto>> AddFavorite(FavoriteCoinDto favoriteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var favorite = await _portfolioService.AddFavoriteAsync(userId, favoriteDto);

            if (favorite == null)
                return BadRequest("Could not add favorite");

            return Ok(favorite);
        }

        [HttpDelete("favorites/{coinId}")]
        public async Task<ActionResult> RemoveFavorite(string coinId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var removed = await _portfolioService.RemoveFavoriteAsync(userId, coinId);

            if (!removed)
                return NotFound("Favorite not found");

            return NoContent();
        }

        // [Authorize]
        // [HttpGet("debug")]
        // public IActionResult Debug()
        // {
        //     var claims = User.Claims.Select(x => new
        //     {
        //        x.Type,
        //        x.Value
        //    });

        //    return Ok(claims);
        // }
    }
    
}

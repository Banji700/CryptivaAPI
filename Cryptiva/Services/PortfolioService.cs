using Cryptiva.Data;
using Cryptiva.Dtos;
using Cryptiva.Interfaces;
using Cryptiva.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cryptiva.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly CryptoPriceService _cryptoPriceService;

        public PortfolioService(ApplicationDbContext dbcontext, CryptoPriceService cryptoPriceService)
        {
            _dbcontext = dbcontext;
            _cryptoPriceService = cryptoPriceService;
        }
        public async Task<PortfolioDto?> GetPortfolioAsync(string userId)
        {
            var portfolio = await _dbcontext.Portfolio
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.AppUserId == userId);

            if (portfolio == null) return null;

            return new PortfolioDto
            {
                CashBalance = portfolio.CashBalance,
                TotalBalance = portfolio.CashBalance
            };
        }

        public async Task<PortfolioDto?> DepositAsync(string userId, decimal amount)
        {
            if(amount <= 0) return null;

            var portfolio = await _dbcontext.Portfolio
            .Include(x => x.Transactions)
            .Include(x => x.Holdings)
            .Include(x => x.Snapshots)
            .FirstOrDefaultAsync(x => x.AppUserId == userId);

            if (portfolio == null) return null;

            portfolio.CashBalance += amount;

           

            portfolio.Transactions.Add(new Transaction
            {
                Type = TransactionType.Deposit,
                Amount = amount,
                PortfolioId = portfolio.Id
            });

            portfolio.Snapshots.Add(new PortfolioSnapshot
            {
                PortfolioId = portfolio.Id,
                TotalValue = await CalculateLiveTotalPortfolioValueAsync(portfolio)
            });


            await _dbcontext.SaveChangesAsync();

            return new PortfolioDto
            {
                CashBalance = portfolio.CashBalance,
                TotalBalance = portfolio.CashBalance
            };
        }

        public async Task<PortfolioDto?> BuyCryptoAsync(string userId, BuyCryptoDto buyCryptoDto)
        {
            if (buyCryptoDto.AmountToSpend <= 0 || buyCryptoDto.CurrentPrice <= 0)
                return null;

            var portfolio = await _dbcontext.Portfolio
            .Include(x => x.Transactions)
            .Include(x => x.Holdings)
            .Include(x => x.Snapshots)
            .FirstOrDefaultAsync(x => x.AppUserId == userId);

            if (portfolio == null) return null;

            if (portfolio.CashBalance < buyCryptoDto.AmountToSpend)
            throw new InvalidOperationException("Insufficient Funds");

            var quantityBought = buyCryptoDto.AmountToSpend / buyCryptoDto.CurrentPrice;

            var holding = portfolio.Holdings.FirstOrDefault(x => x.CoinId == buyCryptoDto.CoinId);

            if(holding == null)
            {
                holding = new CryptoHolding
                {
                    CoinId = buyCryptoDto.CoinId,
                    Symbol = buyCryptoDto.Symbol,
                    Name = buyCryptoDto.Name,
                    Quantity = quantityBought,
                    AverageBuyPrice = buyCryptoDto.CurrentPrice
                };

                portfolio.Holdings.Add(holding);   
            }
            else
            {
                var totalOldValue = holding.Quantity * holding.AverageBuyPrice;
                var totalNewValue = buyCryptoDto.AmountToSpend;
                var newQuantity = holding.Quantity + quantityBought;

                holding.AverageBuyPrice = (totalOldValue + totalNewValue) / newQuantity;
                holding.Quantity = newQuantity;
            }

            portfolio.CashBalance -= buyCryptoDto.AmountToSpend;


            portfolio.Transactions.Add(new Transaction
            {
                Type = TransactionType.Buy,
                CoinId = buyCryptoDto.CoinId,
                Symbol = buyCryptoDto.Symbol,
                Name = buyCryptoDto.Name,
                Amount = buyCryptoDto.AmountToSpend,
                Quantity = quantityBought,
                Price = buyCryptoDto.CurrentPrice,
                PortfolioId = portfolio.Id
            });
            portfolio.Snapshots.Add(new PortfolioSnapshot
            {
                PortfolioId = portfolio.Id,
                TotalValue = await CalculateLiveTotalPortfolioValueAsync(portfolio)
            });

            await _dbcontext.SaveChangesAsync();

            return new PortfolioDto
            {
                CashBalance = portfolio.CashBalance,
                TotalBalance = await CalculateLiveTotalPortfolioValueAsync(portfolio)
            };
        }

        public async Task<IReadOnlyList<HoldingsDto>> GetHoldingsAsync(string userId)
        {
            var portfolio = await _dbcontext.Portfolio.Include(x => x.Holdings).FirstOrDefaultAsync(x => x.AppUserId == userId);

            if (portfolio == null)
                return [];

            return portfolio.Holdings.Select(x => new HoldingsDto
            {
                CoinId = x.CoinId,
                Symbol = x.Symbol,
                Name = x.Name,
                Quantity = x.Quantity,
                AverageBuyPrice = x.AverageBuyPrice,
            }).ToList();
        }

        public async Task<PortfolioDto?> SellCryptoAsync(string userId, SellCryptoDto sellCryptoDto)
        {
            if (sellCryptoDto.QuantityToSell <= 0 || sellCryptoDto.CurrentPrice <= 0)
                return null!;

            var portfolio = await _dbcontext.Portfolio
            .Include(x => x.Transactions)
            .Include(x => x.Holdings)
            .Include(x => x.Snapshots)
            .FirstOrDefaultAsync(x => x.AppUserId == userId);

            if (portfolio == null)
                return null;

            var holding = portfolio.Holdings.FirstOrDefault(x => x.CoinId == sellCryptoDto.CoinId);

            if (holding == null)
                throw new InvalidOperationException("You do not own this crypto");

            if (holding.Quantity < sellCryptoDto.QuantityToSell)
                throw new InvalidOperationException("Insufficient crypto balance");

            var saleValue = sellCryptoDto.QuantityToSell * sellCryptoDto.CurrentPrice;

            holding.Quantity -= sellCryptoDto.QuantityToSell;
            portfolio.CashBalance += saleValue;

            if (holding.Quantity <= 0)
                _dbcontext.CryptoHoldings.Remove(holding);

            portfolio.Transactions.Add(new Transaction
            {
                Type = TransactionType.Sell,
                CoinId = sellCryptoDto.CoinId,
                Symbol = holding.Symbol,
                Name = holding.Name,
                Amount = saleValue,
                Quantity = sellCryptoDto.QuantityToSell,
                Price = sellCryptoDto.CurrentPrice,
                PortfolioId = portfolio.Id
            });

            portfolio.Snapshots.Add(new PortfolioSnapshot
            {
                PortfolioId = portfolio.Id,
                TotalValue = await CalculateLiveTotalPortfolioValueAsync(portfolio)
            });

            await _dbcontext.SaveChangesAsync();

            return new PortfolioDto
            {
                CashBalance = portfolio.CashBalance,
                TotalBalance = portfolio.CashBalance
            };
        }

        public async Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(string userId)
        {
            var portfolio = await _dbcontext.Portfolio
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.AppUserId == userId);

            if (portfolio == null)
                return [];

            return portfolio.Transactions
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new TransactionDto
                {
                    Type = x.Type.ToString(),
                    CoinId = x.CoinId,
                    Symbol = x.Symbol,
                    Name = x.Name,
                    Amount = x.Amount,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    CreatedAt = x.CreatedAt
                })
                .ToList();
        }

        public async Task<IReadOnlyList<SnapshotDto>> GetSnapshotsAsync(string userId)
        {
            var portfolio = await _dbcontext.Portfolio
                .Include(x => x.Snapshots)
                .FirstOrDefaultAsync(x => x.AppUserId == userId);

            if (portfolio == null)
                return [];

            return portfolio.Snapshots
                .OrderBy(x => x.CreatedAt)
                .Select(x => new SnapshotDto
                {
                    TotalValue = x.TotalValue,
                    CreatedAt = x.CreatedAt
                })
                .ToList();
        }

        public async Task<IReadOnlyList<FavoriteCoinDto>> GetFavoritesAsync(string userId)
        {
            var portfolio =  await _dbcontext.Portfolio.Include(x => x.FavoriteCoins).FirstOrDefaultAsync(x => x.AppUserId == userId);

            if (portfolio == null) return [];

            return portfolio.FavoriteCoins.Select(x => new FavoriteCoinDto
            {
                CoinId = x.CoinId,
                Symbol = x.Symbol,
                Name = x.Name,
                Image = x.Image,  

            })
            .ToList();
                

            
        }

        public async Task<FavoriteCoinDto?> AddFavoriteAsync(string userId, FavoriteCoinDto favoriteDto)
        {
            var portfolio = await _dbcontext.Portfolio
       .Include(x => x.FavoriteCoins)
       .FirstOrDefaultAsync(x => x.AppUserId == userId);

            if (portfolio == null)
                return null;

            var alreadyExists = portfolio.FavoriteCoins
                .Any(x => x.CoinId == favoriteDto.CoinId);

            if (alreadyExists)
                return favoriteDto;

            portfolio.FavoriteCoins.Add(new FavoriteCoin
            {
                CoinId = favoriteDto.CoinId,
                Symbol = favoriteDto.Symbol,
                Name = favoriteDto.Name,
                Image = favoriteDto.Image,
                PortfolioId = portfolio.Id
            });

            await _dbcontext.SaveChangesAsync();

            return favoriteDto;
        }

        public async Task<bool> RemoveFavoriteAsync(string userId, string coinId)
        {
            var portfolio = await _dbcontext.Portfolio
       .Include(x => x.FavoriteCoins)
       .FirstOrDefaultAsync(x => x.AppUserId == userId);

            if (portfolio == null)
                return false;

            var favorite = portfolio.FavoriteCoins
                .FirstOrDefault(x => x.CoinId == coinId);

            if (favorite == null)
                return false;

            _dbcontext.FavoriteCoins.Remove(favorite);

            await _dbcontext.SaveChangesAsync();

            return true;
        }

        private async Task<decimal> CalculateLiveTotalPortfolioValueAsync(PortfolioModel portfolio)
        {
            var coinIds = portfolio.Holdings
       .Select(x => x.CoinId)
       .Distinct()
       .ToList();

            var prices = await _cryptoPriceService.GetCurrentPricesAsync(coinIds);

            var holdingsValue = portfolio.Holdings.Sum(holding =>
            {
                var priceToUse = holding.AverageBuyPrice;

                if (prices.TryGetValue(holding.CoinId, out var currentPrice))
                {
                    priceToUse = currentPrice;
                }

                return holding.Quantity * priceToUse;
            });

            return portfolio.CashBalance + holdingsValue;
        }
    }
}

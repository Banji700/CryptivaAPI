using Cryptiva.Dtos;

namespace Cryptiva.Interfaces
{
    public interface IPortfolioService
    {
        Task<PortfolioDto?> GetPortfolioAsync(string userId);

        Task<PortfolioDto?> DepositAsync(string userId, decimal amount);

        Task<PortfolioDto?> BuyCryptoAsync(string userId, BuyCryptoDto buyCryptoDto);

        Task<IReadOnlyList<HoldingsDto>> GetHoldingsAsync(string userId);

        Task<PortfolioDto?> SellCryptoAsync(string userId, SellCryptoDto sellCryptoDto);

        Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(string userId);

        Task<IReadOnlyList<SnapshotDto>> GetSnapshotsAsync(string userId);

        Task<IReadOnlyList<FavoriteCoinDto>> GetFavoritesAsync(string userId);

        Task<FavoriteCoinDto?> AddFavoriteAsync(string userId, FavoriteCoinDto favoriteDto);

        Task<bool> RemoveFavoriteAsync(string userId, string coinId);
    }
}

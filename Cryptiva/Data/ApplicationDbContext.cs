using Cryptiva.Models;
using Cryptiva.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Cryptiva.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        } 

        public DbSet<CryptoHolding>CryptoHoldings { get; set; }
        public DbSet<CryptoTransaction> CryptoTransaction { get; set; }
        public DbSet<PortfolioModel> Portfolio { get; set; }
        public DbSet<WatchListItem> WatchItemList  { get; set; }

        public  DbSet<Transaction> Transactions { get; set; }

        public DbSet<PortfolioSnapshot> PortfolioSnapshots { get; set; }

        public DbSet<FavoriteCoin> FavoriteCoins { get; set; }
    }
};

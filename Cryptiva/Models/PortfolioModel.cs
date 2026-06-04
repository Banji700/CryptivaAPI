using Cryptiva.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptiva.Models
{
    public class PortfolioModel
    {
        public int Id{ get; set; }
        public string AppUserId { get; set; } = string.Empty;

        public AppUser? AppUser { get; set; } //= null!;
        [Column(TypeName = "decimal(18,2)")]
        public decimal CashBalance {  get; set; }

       public List<CryptoHolding> Holdings { get; set; } = [];

        public ICollection<Transaction> Transactions { get; set; } = [];

        public ICollection<PortfolioSnapshot> Snapshots { get; set; } = [];

        public ICollection<FavoriteCoin> FavoriteCoins { get; set; } = [];
    }
}

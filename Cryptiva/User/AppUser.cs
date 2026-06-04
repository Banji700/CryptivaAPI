using Cryptiva.Models;
using Microsoft.AspNetCore.Identity;

namespace Cryptiva.User
{
    public class AppUser: IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public PortfolioModel? PortfolioModel { get; set; }
    }
}

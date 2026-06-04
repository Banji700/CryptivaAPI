using Cryptiva.User;

namespace Cryptiva.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}

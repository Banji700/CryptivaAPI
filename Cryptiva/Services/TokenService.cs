using Cryptiva.Interfaces;
using Cryptiva.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cryptiva.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
           
            
        }
        public Task<string> CreateToken(AppUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName,user.FirstName),
                new Claim(ClaimTypes.Surname,user.LastName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

             var token = new JwtSecurityToken(
             issuer: _config["JWT:Issuer"],
             audience: _config["JWT:Audience"],
             claims: claims,
             expires: DateTime.UtcNow.AddDays(7),
             signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(jwt);
        }
    }
}

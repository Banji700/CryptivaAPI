using Cryptiva.Data;
using Cryptiva.Interfaces;
using Cryptiva.Models;
using Cryptiva.RegisterLogin;
using Cryptiva.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cryptiva.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _dbcontext;

        public AccountController(ITokenService tokenService, UserManager<AppUser> userManager, ApplicationDbContext dbcontext
            )
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _dbcontext = dbcontext;
        }

        [HttpPost("register")]

        public async Task<ActionResult<UserDto>>RegisterUser(RegisterDto registerDto)
        {
            var user = new AppUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

           
            var exsistingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (exsistingUser != null)
            {
                return BadRequest("Email Already Exsists");
            }


            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            };

            var portfolio = new PortfolioModel
            {
                AppUserId = user.Id,
                CashBalance = 10000,
            };
            _dbcontext.Portfolio.Add(portfolio);

            await _dbcontext.SaveChangesAsync();


            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user)
            };
        }

        
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>>LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if(user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result)
            {
                return Unauthorized("Invalid email or password");
            }

            return new UserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName= user.LastName,
                Token = await _tokenService.CreateToken(user)
            };
        }

      //  [Authorize]
       // [HttpGet("test")]
       // public IActionResult TestAuth()
       // {
       //     return Ok("JWT Works");
       // }
    }
}

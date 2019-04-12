using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Fisher.Bookstore.Api.Data;
using Fisher.Bookstore.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;

namespace Fisher.Bookstore.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private IConfiguration configuration;

        public AccountController(UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        IConfiguration configuration)
        {
            this.userManager = userManager; 
            this.signInManager = signInManager;
            this.configuration = configuration; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ApplicationUser registration)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            ApplicationUser user = new ApplicationUser
            {
                Email = registration.Email,
                UserName = registration.Email,
                Id = registration.Email
            };

            IdentityResult result = await userManager.CreateAsync(user, registration.Password);

            if(!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }

                return BadRequest(ModelState);
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApplicationUser login)
        {
            var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent: false, lockoutOnFailure: false); 
            if(!result.Succeeded)
            {
                return Unauthorized();
            }

            ApplicationUser user = await userManager.FindByEmailAsync(login.Email);
            JwtSecurityToken token =  GenerateTokenAsync(user);
            string serializedToken = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new { Token = serializedToken }; 
            return Ok(response);

        }

        private JwtSecurityToken GenerateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var expirationDays = configuration.GetValue<int>
            ("JWTConfiguration: TokenExpirationDays");

            var signingKey = Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWTConfiguration:Key"));

            var token = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("JWTConfiguration:Issuer"),
                audience: configuration.GetValue<string>("JWTConfiguration:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(expirationDays)),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256));
            return token;
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return Ok(User.Identity.Name); 
        }
    }
}
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fisher.Bookstore.Api.Data;
using Fisher.Bookstore.Data;
using Fisher.Bookstore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] ApplicationUser login)
    {
        var result = await SignInManager.PasswordSignInAsync(login.Email,
        login.Password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }
        var response = new { Token = serializedToken};
        return Ok(response);

        ApplicationUser user = await UserManager.FindByEmailAsync(login.Email);
        JwtSecurityToken token = await GenerateTokenAsync(user);
        string serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok();
    }

    IActionResult Unauthorized => throw new NotImplementedException();

    private JwtSecurityToken GenerateToken(ApplicationUser user)
    {
        var claims = new List<Claim>();
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JsonClaimValueTypes.NameIdentifier, user.Id),
            new Claim(JsonClaimValueTypes.Name, user.UserName),
        };

        var expirationDays = congifuration.GetValue<int>
        ("JWTConfiguration:TokenExpirationDays");

        var signingKey = Encoding.UTF8.GetBytes(ConfigurationBinder.GetValue<string>
        ("JWTConfiguration:Key"));

        var token = new JwtSecurityToken(
            IUserSecurityStampStore: configuration.GetValue<string>("JWTConfiguration:Issuer"),
            audience: ConfigurationBinder.GetValue<string>("JWTConfiguration:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(expirationDays)),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(new
            SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256));



        return token;
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return Ok(User.Identity.Name);
    }
}
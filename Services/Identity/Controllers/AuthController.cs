using Identity.DTOs;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, ILogger<AuthController> logger)
        {
            this._userManager = userManager;
            this._config = config;
            this._logger = logger;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            //TODO: Consider Moving this into its own dedicated mapper
            ApplicationUser user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            IdentityResult result = await this._userManager.CreateAsync(user, dto.Password);
            this._logger.LogInformation($"User {dto.Email} registration attempted.");
            if (!result.Succeeded)
                return this.BadRequest(result.Errors);
            return this.Ok(new { message = "Registration Successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            ApplicationUser? user = await this._userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await this._userManager.CheckPasswordAsync(user, dto.Password))
                return this.Unauthorized();

            string token = this.GenerateToken(user);
            this._logger.LogInformation($"User {dto.Email} logged in successfully.");
            return this.Ok(new { token });
        }

        private string GenerateToken(ApplicationUser user)
        {
            Claim[] claims = new[]
            { 
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("uid", user.Id)
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._config["Jwt:Key"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: this._config["Jwt:Issuer"],
                audience: this._config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(this._config["Jwt:DurationInMinutes"])),
                signingCredentials: creds);
            this._logger.LogInformation($"JWT Token generated for user {user.Email}");
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

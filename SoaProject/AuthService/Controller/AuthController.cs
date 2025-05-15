using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _jwtKey;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _jwtKey = _config.GetValue<string>("Jwt:Key") 
                ?? throw new ArgumentNullException("Jwt:Key configuration is missing");
            
            if (_jwtKey.Length < 16)
                throw new ArgumentException("JWT Key must be at least 16 characters");
        }

        [HttpPost("token")]
        public IActionResult GenerateToken([FromBody] LoginRequest request)
        {
            if (request.Username != "admin" || request.Password != "admin")
                return Unauthorized("Invalid credentials");

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtKey));
            
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Administrator")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        public record LoginRequest(string Username, string Password);
    }
}
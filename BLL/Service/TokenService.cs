using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services
{
    public interface ITokenService
    {
        // secondRole: תפקיד נוסף, למשתמש שהוא גם נהג וגם בעל חניה (null = אין)
        AuthResponseDTO CreateToken(int userId, string email, string role, string? secondRole = null);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public AuthResponseDTO CreateToken(int userId, string email, string role, string? secondRole = null)
        {
            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new InvalidOperationException("Jwt:Key is missing. Set the environment variable Jwt__Key.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            // משתמש שהוא גם נהג וגם בעל חניה: מוסיפים לטוקן גם את התפקיד השני.
            // כך [Authorize(Roles = "Owner")] וגם [Authorize(Roles = "Driver")] יעבדו לו
            if (secondRole != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, secondRole));
            }

            var minutes = int.Parse(_config["Jwt:ExpiresMinutes"] ?? "60");
            var expires = DateTime.UtcNow.AddMinutes(minutes);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new AuthResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expires,
                Role = role,
                UserId = userId
            };
        }
    }
}
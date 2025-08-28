using HotPot23API.Interfaces;
using HotPot23API.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotPot23API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            string secret = configuration["Tokens:JWT"] ?? string.Empty;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public async Task<string> GenerateToken(TokenUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

          
            if (user.RestaurantID.HasValue)
            {
                claims.Add(new Claim("RestaurantId", user.RestaurantID.Value.ToString()));
            }
            if (user.UserID.HasValue)
            {
                claims.Add(new Claim("UserId", user.UserID.Value.ToString()));
            }

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

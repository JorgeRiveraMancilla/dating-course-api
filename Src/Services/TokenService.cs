using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dating_course_api.Src.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace dating_course_api.Src.Services
{
    public class TokenService(IConfiguration config) : ITokenService
    {
        private readonly SymmetricSecurityKey _key =
            new(
                Encoding.UTF8.GetBytes(
                    config["JWTSettings:TokenKey"]
                        ?? throw new InvalidOperationException("TokenKey not found")
                )
            );

        public string CreateToken(int userId, string userEmail)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, userId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, userEmail)
            };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

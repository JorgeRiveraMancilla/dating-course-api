using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dating_course_api.Src.Entities;
using dating_course_api.Src.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace dating_course_api.Src.Services
{
    public class TokenService(IConfiguration config, UserManager<User> userManager) : ITokenService
    {
        public async Task<string> CreateTokenAsync(int userId, string userEmail)
        {
            var tokenKey =
                config["JWTSettings:TokenKey"] ?? throw new Exception("Token key not found");

            if (tokenKey.Length < 64)
                throw new Exception("Token key must be at least 64 characters long");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, userEmail)
            };

            var user =
                await userManager.FindByIdAsync(userId.ToString())
                ?? throw new Exception("User not found");
            var roles = await userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

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

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookManager.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace BookManager.API.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            }),
            
            Expires = DateTime.UtcNow.AddHours(2),
  
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
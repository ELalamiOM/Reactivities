using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using Application.Interfaces;

namespace API.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public DateTime GetRefreshTokenExpiryDate()
    {
        var expiresInDays = configuration.GetValue<int?>("JwtSettings:RefreshTokenExpiresInDays") ?? 7;
        return DateTime.UtcNow.AddDays(expiresInDays);
    }

    public string CreateToken(User user)
    {
        var key = configuration["JwtSettings:Key"]
            ?? throw new InvalidOperationException("JwtSettings:Key is not configured");
        var issuer = configuration["JwtSettings:Issuer"]
            ?? throw new InvalidOperationException("JwtSettings:Issuer is not configured");
        var audience = configuration["JwtSettings:Audience"]
            ?? throw new InvalidOperationException("JwtSettings:Audience is not configured");
        var expiresInMinutes = configuration.GetValue<int?>("JwtSettings:ExpiresInMinutes") ?? 60;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.DisplayName ?? user.UserName ?? string.Empty)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256Signature
        );

        var tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}

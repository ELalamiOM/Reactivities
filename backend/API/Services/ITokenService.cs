using Domain;

namespace API.Services;

public interface ITokenService
{
    string CreateToken(User user);
    string GenerateRefreshToken();
    DateTime GetRefreshTokenExpiryDate();
}

using Application.Account.DTOs;

namespace Application.Account.Common;

public class AccountAuthResult
{
    public required UserDto User { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime RefreshTokenExpiresAt { get; set; }
}

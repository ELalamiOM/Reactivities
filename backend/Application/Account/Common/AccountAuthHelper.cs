using Application.Core;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Account.Common;

public static class AccountAuthHelper
{
    public static async Task<Result<AccountAuthResult>> CreateAndPersistUserAuthAsync(
        User user,
        UserManager<User> userManager,
        ITokenService tokenService,
        ILogger logger)
    {
        user.RefreshToken = tokenService.GenerateRefreshToken();
        user.RefreshTokenExpiresAt = tokenService.GetRefreshTokenExpiryDate();

        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            logger.LogError("Failed to persist refresh token for user: {UserId}", user.Id);
            return Result<AccountAuthResult>.Failure("Could not persist refresh token for the user", 400);
        }

        return Result<AccountAuthResult>.Success(new AccountAuthResult
        {
            User = new()
            {
                Id = user.Id,
                DisplayName = user.DisplayName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ImageUrl = user.ImageUrl,
                Token = tokenService.CreateToken(user)
            },
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiresAt = user.RefreshTokenExpiresAt
        });
    }
}

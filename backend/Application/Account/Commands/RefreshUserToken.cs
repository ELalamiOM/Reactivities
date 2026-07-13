using Application.Account.Common;
using Application.Core;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Account.Commands;

public class RefreshUserToken
{
    public class Command : IRequest<Result<AccountAuthResult>>
    {
        public required string RefreshToken { get; set; }
    }

    public class Handler(
        SignInManager<User> signInManager,
        ITokenService tokenService,
        ILogger<Handler> logger)
        : IRequestHandler<Command, Result<AccountAuthResult>>
    {
        public async Task<Result<AccountAuthResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Refresh token request received");

            var user = await signInManager.UserManager.Users
                .FirstOrDefaultAsync(x => x.RefreshToken == request.RefreshToken, cancellationToken);

            if (user == null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                logger.LogWarning("Refresh token invalid or expired");
                return Result<AccountAuthResult>.Failure("Refresh token is invalid or expired", 401);
            }

            logger.LogInformation("Token refreshed successfully for user: {UserId}", user.Id);
            return await AccountAuthHelper.CreateAndPersistUserAuthAsync(
                user,
                signInManager.UserManager,
                tokenService,
                logger);
        }
    }
}

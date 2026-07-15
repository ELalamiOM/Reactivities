using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Account.Commands;

// Service de déconnexion utilisateur
// Révoque le refresh token en base de données
// Le cookie est supprimé côté contrôleur
public class LogoutUser
{
    public class Command : IRequest<Result<Unit>>
    {
        public string? RefreshToken { get; set; }
    }

    public class Handler(SignInManager<User> signInManager, ILogger<Handler> logger)
        : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Logout request received");

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Result<Unit>.Success(Unit.Value);
            }

            var user = await signInManager.UserManager.Users
                .FirstOrDefaultAsync(x => x.RefreshToken == request.RefreshToken, cancellationToken);

            if (user == null)
            {
                return Result<Unit>.Success(Unit.Value);
            }

            logger.LogInformation("Revoking refresh token for user: {UserId}", user.Id);
            user.RefreshToken = null;
            user.RefreshTokenExpiresAt = DateTime.UtcNow;

            var updateResult = await signInManager.UserManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                logger.LogError("Failed to revoke refresh token for user: {UserId}", user.Id);
                return Result<Unit>.Failure("Could not revoke refresh token", 400);
            }

            logger.LogInformation("User logged out successfully");
            return Result<Unit>.Success(Unit.Value);
        }
    }
}

using Application.Account.Common;
using Application.Core;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Account.Queries;

public class GetCurrentUserInfo
{
    public class Query : IRequest<Result<AccountAuthResult>>
    {
        public required string UserId { get; set; }
    }

    public class Handler(
        SignInManager<User> signInManager,
        ITokenService tokenService,
        ILogger<Handler> logger)
        : IRequestHandler<Query, Result<AccountAuthResult>>
    {
        public async Task<Result<AccountAuthResult>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await signInManager.UserManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                logger.LogWarning("User info request - user not found");
                return Result<AccountAuthResult>.Failure("User not found", 401);
            }

            logger.LogDebug("User info retrieved for: {UserId}", user.Id);
            return await AccountAuthHelper.CreateAndPersistUserAuthAsync(
                user,
                signInManager.UserManager,
                tokenService,
                logger);
        }
    }
}

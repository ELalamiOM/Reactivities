using Application.Account.DTOs;
using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Account.Commands;

public class ForgotPassword
{
    public class Command : IRequest<Result<Unit>>
    {
        public required ForgotPasswordDto ForgotPasswordDto { get; set; }
    }

    public class Handler(SignInManager<User> signInManager, ILogger<Handler> logger)
        : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Forgot password request for email: {Email}", request.ForgotPasswordDto.Email);

            var user = await signInManager.UserManager.FindByEmailAsync(request.ForgotPasswordDto.Email);

            if (user == null)
            {
                logger.LogDebug("Forgot password - user not found (returning OK for security): {Email}", request.ForgotPasswordDto.Email);
                return Result<Unit>.Success(Unit.Value);
            }

            logger.LogInformation("Password reset initiated for user: {UserId}", user.Id);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}

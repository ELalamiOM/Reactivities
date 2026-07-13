using Application.Account.Common;
using Application.Account.DTOs;
using Application.Core;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Account.Commands;

public class LoginUser
{
    public class Command : IRequest<Result<AccountAuthResult>>
    {
        public required LoginDto LoginDto { get; set; }
    }

    public class Handler(
        SignInManager<User> signInManager,
        ITokenService tokenService,
        ILogger<Handler> logger)
        : IRequestHandler<Command, Result<AccountAuthResult>>
    {
        public async Task<Result<AccountAuthResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Login attempt for email: {Email}", request.LoginDto.Email);

            var user = await signInManager.UserManager.FindByEmailAsync(request.LoginDto.Email);
            if (user == null)
            {
                logger.LogWarning("Login failed - user not found: {Email}", request.LoginDto.Email);
                return Result<AccountAuthResult>.Failure("Invalid email or password", 401);
            }

            var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.LoginDto.Password, false);
            if (!signInResult.Succeeded)
            {
                logger.LogWarning("Login failed - invalid password for user: {Email}", request.LoginDto.Email);
                return Result<AccountAuthResult>.Failure("Invalid email or password", 401);
            }

            logger.LogInformation("User logged in successfully: {Email}, UserId: {UserId}", request.LoginDto.Email, user.Id);
            return await AccountAuthHelper.CreateAndPersistUserAuthAsync(
                user,
                signInManager.UserManager,
                tokenService,
                logger);
        }
    }
}

using Application.Account.Common;
using Application.Account.DTOs;
using Application.Core;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Account.Commands;

public class RegisterUser
{
    public class Command : IRequest<Result<AccountAuthResult>>
    {
        public required RegisterDto RegisterDto { get; set; }
    }

    public class Handler(
        SignInManager<User> signInManager,
        ITokenService tokenService,
        AppDbContext context,
        ILogger<Handler> logger)
        : IRequestHandler<Command, Result<AccountAuthResult>>
    {
        public async Task<Result<AccountAuthResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Registration attempt for email: {Email}", request.RegisterDto.Email);

            var user = new User
            {
                UserName = request.RegisterDto.Email,
                Email = request.RegisterDto.Email,
                DisplayName = request.RegisterDto.DisplayName
            };

            var result = await signInManager.UserManager.CreateAsync(user, request.RegisterDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogWarning("Registration failed for email: {Email}, Errors: {Errors}", request.RegisterDto.Email, errors);
                return Result<AccountAuthResult>.Failure(errors, 400);
            }

            var prepaidAccount = new PrepaidAccount
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Balance = 100
            };

            var initialTransaction = new AccountTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = prepaidAccount.Id,
                Type = TransactionType.InitialCredit,
                Amount = 100,
                BalanceBefore = 0,
                BalanceAfter = 100,
                IdempotencyKey = $"initial-credit:{user.Id}",
                CreatedAt = DateTime.UtcNow
            };

            context.PrepaidAccounts.Add(prepaidAccount);
            context.AccountTransactions.Add(initialTransaction);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User registered successfully: {Email}, UserId: {UserId}", request.RegisterDto.Email, user.Id);
            return await AccountAuthHelper.CreateAndPersistUserAuthAsync(
                user,
                signInManager.UserManager,
                tokenService,
                logger);
        }
    }
}

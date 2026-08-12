using Application.Core;
using Application.Interfaces;
using Application.PrepaidAccounts.DTOs;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PrepaidAccounts.Queries;

public class GetPrepaidAccount
{
    public class Query : IRequest<Result<PrepaidAccountDto>> { }

    public class Handler(AppDbContext context, IUserAccessor userAccessor)
        : IRequestHandler<Query, Result<PrepaidAccountDto>>
    {
        public async Task<Result<PrepaidAccountDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await userAccessor.GetUserAsync();

            var account = await context.PrepaidAccounts
                .Include(x => x.Transactions.OrderByDescending(t => t.CreatedAt))
                    .ThenInclude(t => t.Activity)
                .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

            if (account == null)
                return Result<PrepaidAccountDto>.Failure("Compte prépayé introuvable", 404);

            var dto = new PrepaidAccountDto
            {
                Balance = account.Balance,
                Transactions = account.Transactions.Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Type = t.Type.ToString(),
                    Amount = t.Amount,
                    BalanceBefore = t.BalanceBefore,
                    BalanceAfter = t.BalanceAfter,
                    ActivityId = t.ActivityId,
                    ActivityTitle = t.Activity?.Title,
                    CreatedAt = t.CreatedAt
                }).ToList()
            };

            return Result<PrepaidAccountDto>.Success(dto);
        }
    }
}

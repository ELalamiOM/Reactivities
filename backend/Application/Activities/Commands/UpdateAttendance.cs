using Application.Core;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities.Commands;

public class UpdateAttendance
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string Id { get; set; }
    }

    public class Handler(AppDbContext context, IUserAccessor userAccessor)
        : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .Include(x => x.Attendees)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (activity == null) return Result<Unit>.Failure("Activity not found", 404);

            var user = await userAccessor.GetUserAsync();

            var attendance = activity.Attendees
                .FirstOrDefault(x => x.UserId == user.Id);

            var isHost = activity.Attendees.Any(x => x.IsHost && x.UserId == user.Id);

            var price = activity.Price ?? 0;

            if (attendance != null)
            {
                if (isHost)
                {
                    activity.IsCancelled = !activity.IsCancelled;
                }
                else
                {
                    if (price > 0)
                    {
                        var refundResult = await RefundAsync(user.Id, activity.Id, price, cancellationToken);
                        if (!refundResult.IsSuccess)
                            return refundResult;
                    }

                    activity.Attendees.Remove(attendance);
                }
            }
            else
            {
                if (activity.IsCancelled)
                    return Result<Unit>.Failure("Impossible de s'inscrire à une activité annulée", 400);

                if (price > 0)
                {
                    var debitResult = await DebitAsync(user.Id, activity.Id, price, cancellationToken);
                    if (!debitResult.IsSuccess)
                        return debitResult;
                }

                activity.Attendees.Add(new ActivityAttendee
                {
                    UserId = user.Id,
                    IsHost = false
                });
            }

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Unit>.Success(Unit.Value)
                : Result<Unit>.Failure("Problem updating the attendance", 400);
        }

        private async Task<Result<Unit>> DebitAsync(string userId, string activityId, decimal price, CancellationToken cancellationToken)
        {
            var attemptCount = await context.AccountTransactions
                .CountAsync(x => x.ActivityId == activityId
                    && x.Account.UserId == userId
                    && x.Type == TransactionType.Debit, cancellationToken);

            var idempotencyKey = $"debit:{userId}:{activityId}:{attemptCount + 1}";

            var existingTransaction = await context.AccountTransactions
                .AnyAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);

            if (existingTransaction)
                return Result<Unit>.Failure("Cette inscription a déjà été traitée", 400);

            var account = await context.PrepaidAccounts
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (account == null)
                return Result<Unit>.Failure("Compte prépayé introuvable", 400);

            if (account.Balance < price)
                return Result<Unit>.Failure("Solde insuffisant", 400);

            var balanceBefore = account.Balance;
            account.Balance -= price;

            context.AccountTransactions.Add(new AccountTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                Type = TransactionType.Debit,
                Amount = -price,
                BalanceBefore = balanceBefore,
                BalanceAfter = account.Balance,
                ActivityId = activityId,
                IdempotencyKey = idempotencyKey,
                CreatedAt = DateTime.UtcNow
            });

            return Result<Unit>.Success(Unit.Value);
        }

        private async Task<Result<Unit>> RefundAsync(string userId, string activityId, decimal price, CancellationToken cancellationToken)
        {
            var attemptCount = await context.AccountTransactions
                .CountAsync(x => x.ActivityId == activityId
                    && x.Account.UserId == userId
                    && x.Type == TransactionType.Refund, cancellationToken);

            var idempotencyKey = $"refund:{userId}:{activityId}:{attemptCount + 1}";

            var existingTransaction = await context.AccountTransactions
                .AnyAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);

            if (existingTransaction)
                return Result<Unit>.Success(Unit.Value);

            var account = await context.PrepaidAccounts
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (account == null)
                return Result<Unit>.Failure("Compte prépayé introuvable", 400);

            var balanceBefore = account.Balance;
            account.Balance += price;

            context.AccountTransactions.Add(new AccountTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                Type = TransactionType.Refund,
                Amount = price,
                BalanceBefore = balanceBefore,
                BalanceAfter = account.Balance,
                ActivityId = activityId,
                IdempotencyKey = idempotencyKey,
                CreatedAt = DateTime.UtcNow
            });

            return Result<Unit>.Success(Unit.Value);
        }
    }
}

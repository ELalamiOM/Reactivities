using Application.Core;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Inscriptions.Commands;

public class Unregister
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string ActivityId { get; set; }
    }

    public class Handler(AppDbContext context, IUserAccessor userAccessor)
        : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activity == null)
                return Result<Unit>.Failure("Activité non trouvée", 404);

            var user = await userAccessor.GetUserAsync();

            var attendance = activity.Attendees
                .FirstOrDefault(x => x.UserId == user.Id);

            if (attendance == null)
                return Result<Unit>.Failure("Vous n'êtes pas inscrit à cette activité", 400);

            if (attendance.IsHost)
                return Result<Unit>.Failure("L'hôte ne peut pas se désinscrire. Annulez l'activité à la place.", 403);

            var price = activity.Price ?? 0;

            if (price > 0)
            {
                 var attemptCount = await context.AccountTransactions
    .CountAsync(x => x.ActivityId == activity.Id
                  && x.Account.UserId == user.Id
                  && x.Type == TransactionType.Refund, cancellationToken);

                 var idempotencyKey = $"refund:{user.Id}:{activity.Id}:{attemptCount + 1}";
             //   var idempotencyKey = $"refund:{user.Id}:{activity.Id}";

                var existingTransaction = await context.AccountTransactions
                    .AnyAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);

                if (!existingTransaction)
                {
                    var account = await context.PrepaidAccounts
                        .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

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
                        ActivityId = activity.Id,
                        IdempotencyKey = idempotencyKey,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            activity.Attendees.Remove(attendance);

            try
            {
                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                return result
                    ? Result<Unit>.Success(Unit.Value)
                    : Result<Unit>.Failure("Problème lors de la désinscription", 400);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<Unit>.Failure("Conflit de concurrence. Veuillez réessayer.", 400);
            }
        }
    }
}

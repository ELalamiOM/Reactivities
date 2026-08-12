using Application.Core;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Inscriptions.Commands;

public class Register
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

            if (activity.IsCancelled)
                return Result<Unit>.Failure("Impossible de s'inscrire à une activité annulée", 400);

            var user = await userAccessor.GetUserAsync();

            var existingAttendance = activity.Attendees
                .FirstOrDefault(x => x.UserId == user.Id);

            if (existingAttendance != null)
                return Result<Unit>.Failure("Vous êtes déjà inscrit à cette activité", 400);

            var price = activity.Price ?? 0;

            if (price > 0)
            {
                var attemptCount = await context.AccountTransactions
                     .CountAsync(x => x.ActivityId == activity.Id
                  && x.Account.UserId == user.Id
                  && x.Type == TransactionType.Debit, cancellationToken);

                var idempotencyKey = $"debit:{user.Id}:{activity.Id}:{attemptCount + 1}";
               // var idempotencyKey = $"debit:{user.Id}:{activity.Id}";

                var existingTransaction = await context.AccountTransactions
                    .AnyAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);

                if (existingTransaction)
                    return Result<Unit>.Failure("Cette inscription a déjà été traitée", 400);

                var account = await context.PrepaidAccounts
                    .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

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
                    ActivityId = activity.Id,
                    IdempotencyKey = idempotencyKey,
                    CreatedAt = DateTime.UtcNow
                });
            }

            activity.Attendees.Add(new ActivityAttendee
            {
                UserId = user.Id,
                ActivityId = request.ActivityId,
                IsHost = false,
                DateJoined = DateTime.UtcNow
            });

            try
            {
                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                return result
                    ? Result<Unit>.Success(Unit.Value)
                    : Result<Unit>.Failure("Problème lors de l'inscription", 400);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<Unit>.Failure("Conflit de concurrence. Veuillez réessayer.", 400);
            }
        }
    }
}

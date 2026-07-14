using Application.Core;
using Application.Interfaces;
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

            activity.Attendees.Remove(attendance);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Unit>.Success(Unit.Value)
                : Result<Unit>.Failure("Problème lors de la désinscription", 400);
        }
    }
}

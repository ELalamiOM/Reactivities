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

            activity.Attendees.Add(new ActivityAttendee
            {
                UserId = user.Id,
                ActivityId = request.ActivityId,
                IsHost = false,
                DateJoined = DateTime.UtcNow
            });

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Unit>.Success(Unit.Value)
                : Result<Unit>.Failure("Problème lors de l'inscription", 400);
        }
    }
}

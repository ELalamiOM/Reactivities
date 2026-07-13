using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Core;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Activities.Commands;

public class DeleteActivity
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string Id { get; set; }
    }

    public class Handler(AppDbContext context, IUserAccessor userAccessor, ILogger<Handler> logger)
        : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Attempting to delete activity: {ActivityId}", request.Id);
            
            var activity = await context.Activities
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (activity == null)
            {
                logger.LogWarning("Activity not found for deletion: {ActivityId}", request.Id);
                return Result<Unit>.Failure("Activity not found", 404);
            }

            var userId = userAccessor.GetUserId();
            var isHost = activity.Attendees.Any(x => x.UserId == userId && x.IsHost);

            if (!isHost)
            {
                logger.LogWarning("User {UserId} attempted to delete activity {ActivityId} without being host", userId, request.Id);
                return Result<Unit>.Failure("Only the host can delete this activity", 403);
            }

            context.Remove(activity);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                logger.LogError("Failed to delete activity: {ActivityId}", request.Id);
                return Result<Unit>.Failure("Failed to delete activity", 400);
            }

            logger.LogInformation("Activity deleted successfully: {ActivityId} by user {UserId}", request.Id, userId);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}

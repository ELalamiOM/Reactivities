using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Application.Core;
using Application.Activities.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Activities.Commands;

public class EditActivity
{
    public class Command : IRequest<Result<Unit>>
    {
        public required EditActivityDto ActivityDto { get; set; }
    }

    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor, ILogger<Handler> logger)
        : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Attempting to edit activity: {ActivityId}", request.ActivityDto.Id);
            
            var activity = await context.Activities
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == request.ActivityDto.Id, cancellationToken);

            if (activity == null)
            {
                logger.LogWarning("Activity not found for editing: {ActivityId}", request.ActivityDto.Id);
                return Result<Unit>.Failure("Activity not found", 404);
            }

            var userId = userAccessor.GetUserId();
            var isHost = activity.Attendees.Any(x => x.UserId == userId && x.IsHost);

            if (!isHost)
            {
                logger.LogWarning("User {UserId} attempted to edit activity {ActivityId} without being host", userId, request.ActivityDto.Id);
                return Result<Unit>.Failure("Only the host can edit this activity", 403);
            }

            mapper.Map(request.ActivityDto, activity);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                logger.LogError("Failed to edit activity: {ActivityId}", request.ActivityDto.Id);
                return Result<Unit>.Failure("Failed to edit activity", 400);
            }

            logger.LogInformation("Activity edited successfully: {ActivityId} by user {UserId}", request.ActivityDto.Id, userId);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
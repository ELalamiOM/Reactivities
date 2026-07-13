using System;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Activities.DTOs;
using AutoMapper;
using FluentValidation;
using Application.Core;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Activities.Commands;

public class CreateActivity
{
    public class Command : IRequest<Result<string>>
    {
        public required CreateActivityDto ActivityDto {get; set;}
    }

    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor, ILogger<Handler> logger)
        : IRequestHandler<Command ,Result<string>>
    {
        public async Task<Result<string>> Handle(Command request,CancellationToken cancellationToken)
        {
            logger.LogInformation("Creating new activity: {Title}", request.ActivityDto.Title);
            
            var user = await userAccessor.GetUserAsync();
            logger.LogDebug("Activity created by user: {UserId}", user.Id);

            var activity = mapper.Map<Activity>(request.ActivityDto);

            activity.Attendees.Add(new ActivityAttendee
            {
                UserId = user.Id,
                IsHost = true
            });

            context.Activities.Add(activity);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            if(!result)
            {
                logger.LogWarning("Failed to create activity: {Title}", request.ActivityDto.Title);
                return Result<string>.Failure("Failed to create activity",400);
            }

            logger.LogInformation("Activity created successfully with ID: {ActivityId}", activity.Id);
            return Result<string>.Success(activity.Id);

        }
    }
}
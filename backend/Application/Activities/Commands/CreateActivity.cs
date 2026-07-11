using System;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Activities.DTOs;
using AutoMapper;
using FluentValidation;
using Application.Core;
using Application.Interfaces;

namespace Application.Activities.Commands;

public class CreateActivity
{
    public class Command : IRequest<Result<string>>
    {
        public required CreateActivityDto ActivityDto {get; set;}
    }

    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor)
        : IRequestHandler<Command ,Result<string>>
    {
        public async Task<Result<string>> Handle(Command request,CancellationToken cancellationToken)
        {
          var user = await userAccessor.GetUserAsync();

          var activity = mapper.Map<Activity>(request.ActivityDto);

          activity.Attendees.Add(new ActivityAttendee
          {
              UserId = user.Id,
              IsHost = true
          });

          context.Activites.Add(activity);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            if(!result) return Result<string>.Failure("Failed to create activity",400);

            return Result<string>.Success(activity.Id);

        }
    }
}
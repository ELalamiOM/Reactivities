using System;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Activities.DTOs;
using AutoMapper;
using FluentValidation;
using Application.Core;

namespace Application.Activities.Commands;

public class CreateActivity
{
    public class Command : IRequest<Result<string>>
    {
        public required CreateActivityDto ActivityDto {get; set;}
    }

    public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Command ,Result<string>>
    {
        public async Task<Result<string>> Handle(Command request,CancellationToken cancellationToken)
        {
          // await validator.ValidateAndThrowAsync(request.ActivityDto, cancellationToken);
          var activity = mapper.Map<Activity>(request.ActivityDto);
          context.Activites.Add(activity);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            if(!result) return Result<string>.Failure("Failed to create activity",400);

            return Result<string>.Success(activity.Id);

        }
    }
}
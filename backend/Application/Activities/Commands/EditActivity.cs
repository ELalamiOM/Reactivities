using System;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Application.Core;

namespace Application.Activities.Commands;

public class EditActivity
{
    public class Command : IRequest<Result<Unit>>
    {
        public required Activity Activity {get; set;}
    }

    public class Handler(AppDbContext context,IMapper mapper) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activites.FindAsync([request.Activity.Id],cancellationToken);
            
            if(activity == null) return Result<Unit>.Failure("Activity not found",404);

            mapper.Map(request.Activity, activity);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            if(!result) return Result<Unit>.Failure("Failed to edit activity",400);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
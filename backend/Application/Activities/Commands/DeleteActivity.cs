using System;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Core;

namespace Application.Activities.Commands;

public class DeleteActivity
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string Id {get; set;}
    }

    public class Handler(AppDbContext context) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
          var activity = await context.Activites
              .FindAsync([request.Id],cancellationToken);

         if(activity == null) return Result<Unit>.Failure("Activity not found",404);
          
          context.Remove(activity);

          var result = await context.SaveChangesAsync(cancellationToken) > 0;
          
          if(!result) return Result<Unit>.Failure("Failed to delete activity",400);
          return Result<Unit>.Success(Unit.Value);
        }
    }
}

using System;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities.Commands;

public class DeleteActivity
{
    public class Command : IRequest
    {
        public required string Id {get; set;}
    }

    public class Handler(AppDbContext context) : IRequestHandler<Command>
    {
        public async Task Handle(Command request,CancellationToken cancellationToken)
        {
          var activity = await context.Activites
              .FindAsync([request.Id],cancellationToken)
              ?? throw new Exception("Cannot find activity");
          
          context.Remove(activity);

          await context.SaveChangesAsync(cancellationToken);

        }
    }
}
using System;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Application.Activities.Commands;

public class EditActivity
{
    public class Command : IRequest
    {
        public required Activity Activity {get; set;}
    }

    public class Handler(AppDbContext context,IMapper mapper) : IRequestHandler<Command>
    {
        public async Task Handle(Command request,CancellationToken cancellationToken)
        {
            var activity = await context.Activites.FindAsync([request.Activity.Id],cancellationToken)
                  ?? throw new Exception("Cannot find activity");
            
            mapper.Map(request.Activity, activity);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
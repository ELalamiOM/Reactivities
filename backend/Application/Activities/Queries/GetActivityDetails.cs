using System;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Core;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    public class Query : IRequest<Result<Activity>>
    {
        public required string Id {get; set;}
    }

    public class Handler(AppDbContext context) : IRequestHandler<Query ,Result<Activity>>
    {
        public async Task<Result<Activity>> Handle(Query request,CancellationToken cancellationToken)
        {
            var activity = await context.Activites.FindAsync([request.Id], cancellationToken);

            if(activity == null) return Result<Activity>.Failure("Activity not found",404);

             return Result<Activity>.Success(activity);
           // return await context.Activites.ToListAsync(cancellationToken);
        }
    }
}
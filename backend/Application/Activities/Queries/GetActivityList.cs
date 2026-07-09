using System;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Activities.Queries;

public class GetActivityList
{
    public class Query : IRequest<List<Activity>> {}

    public class Handler(AppDbContext context) : IRequestHandler<Query ,List<Activity>>
    {
        public async Task<List<Activity>> Handle(Query request,CancellationToken cancellationToken)
        {
           /* try
            {
                for (int i=0; i < 10;i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Delay(1000,cancellationToken);
                    logger.LogInformation($"Task {i} has completed");
                }
            }
            catch(System.Exception)
            {
                logger.LogInformation($"Task was completed");
            } */

            return await context.Activites.ToListAsync(cancellationToken);
        }
    }
}
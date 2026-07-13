using System;
using Domain;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Core;
using Application.Activities.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    public class Query : IRequest<Result<ActivityDto>>
    {
        public required string Id {get; set;}
    }

    public class Handler(AppDbContext context, IMapper mapper, ILogger<Handler> logger)
        : IRequestHandler<Query ,Result<ActivityDto>>
    {
        public async Task<Result<ActivityDto>> Handle(Query request,CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching activity details for: {ActivityId}", request.Id);
            
            var activity = await context.Activities
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if(activity == null)
            {
                logger.LogWarning("Activity not found: {ActivityId}", request.Id);
                return Result<ActivityDto>.Failure("Activity not found",404);
            }

            logger.LogDebug("Activity retrieved successfully: {ActivityId} - {Title}", activity.Id, activity.Title);
            return Result<ActivityDto>.Success(activity);
        }
    }
}
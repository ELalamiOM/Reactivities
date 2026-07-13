using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Activities.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;

namespace Application.Activities.Queries;

public class GetActivityList
{
    public class Query : IRequest<Result<PagedList<ActivityDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Filter { get; set; }
    }

    public class Handler(AppDbContext context, IMapper mapper, ILogger<Handler> logger)
        : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
    {
        public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Fetching activities - Page: {PageNumber}, Size: {PageSize}, Filter: {Filter}", 
                request.PageNumber, request.PageSize, request.Filter ?? "none");
            
            var query = context.Activities
                .OrderByDescending(x => x.Date)
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                .AsQueryable();

            if (request.Filter == "upcoming")
                query = query.Where(x => x.Date >= DateTime.UtcNow);
            else if (request.Filter == "past")
                query = query.Where(x => x.Date < DateTime.UtcNow);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var pagedList = new PagedList<ActivityDto>(items, totalCount, request.PageNumber, request.PageSize);

            logger.LogDebug("Retrieved {Count} activities out of {TotalCount} total", items.Count, totalCount);
            return Result<PagedList<ActivityDto>>.Success(pagedList);
        }
    }
}
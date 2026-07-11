using System;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Activities.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Application.Activities.Queries;

public class GetActivityList
{
    public class Query : IRequest<List<ActivityDto>> {}

    public class Handler(AppDbContext context, IMapper mapper)
        : IRequestHandler<Query ,List<ActivityDto>>
    {
        public async Task<List<ActivityDto>> Handle(Query request,CancellationToken cancellationToken)
        {
            return await context.Activites
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}
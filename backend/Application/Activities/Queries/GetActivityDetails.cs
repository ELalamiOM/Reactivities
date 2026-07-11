using System;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Core;
using Application.Activities.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    public class Query : IRequest<Result<ActivityDto>>
    {
        public required string Id {get; set;}
    }

    public class Handler(AppDbContext context, IMapper mapper)
        : IRequestHandler<Query ,Result<ActivityDto>>
    {
        public async Task<Result<ActivityDto>> Handle(Query request,CancellationToken cancellationToken)
        {
            var activity = await context.Activites
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if(activity == null) return Result<ActivityDto>.Failure("Activity not found",404);

             return Result<ActivityDto>.Success(activity);
        }
    }
}
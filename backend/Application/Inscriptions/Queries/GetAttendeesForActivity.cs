using Application.Core;
using Application.Inscriptions.DTOs;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Inscriptions.Queries;

public class GetAttendeesForActivity
{
    public class Query : IRequest<Result<List<AttendeeDto>>>
    {
        public required string ActivityId { get; set; }
    }

    public class Handler(AppDbContext context)
        : IRequestHandler<Query, Result<List<AttendeeDto>>>
    {
        public async Task<Result<List<AttendeeDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .Include(x => x.Attendees)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activity == null)
                return Result<List<AttendeeDto>>.Failure("Activité non trouvée", 404);

            var attendees = activity.Attendees
                .Select(a => new AttendeeDto
                {
                    UserId = a.UserId ?? "",
                    DisplayName = a.User.DisplayName ?? "",
                    ImageUrl = a.User.ImageUrl,
                    IsHost = a.IsHost,
                    DateJoined = a.DateJoined
                })
                .ToList();

            return Result<List<AttendeeDto>>.Success(attendees);
        }
    }
}

using AutoMapper;
using Application.Activities.DTOs;
using Domain.Entities;

namespace Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateActivityDto, Activity>();
        CreateMap<EditActivityDto, Activity>();

        CreateMap<Activity, ActivityDto>()
            .ForMember(d => d.HostId, o => o.MapFrom(s =>
                s.Attendees.FirstOrDefault(x => x.IsHost)!.UserId))
            .ForMember(d => d.HostDisplayName, o => o.MapFrom(s =>
                s.Attendees.FirstOrDefault(x => x.IsHost)!.User.DisplayName));

        CreateMap<ActivityAttendee, UserProfileDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.User.Id))
            .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.User.DisplayName))
            .ForMember(d => d.Bio, o => o.MapFrom(s => s.User.Bio))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.User.ImageUrl));
    }
}
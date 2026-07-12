using System;
using Microsoft.AspNetCore.Identity;

namespace Domain;

public class User : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }

    // navigation
    public ICollection<ActivityAttendee> Activities { get; set; } = [];

    public ICollection<Photo> Photos { get; set; } = [];
}
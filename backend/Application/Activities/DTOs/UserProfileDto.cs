namespace Application.Activities.DTOs;

public class UserProfileDto
{
    public string Id { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }
}

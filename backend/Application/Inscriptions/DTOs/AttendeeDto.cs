namespace Application.Inscriptions.DTOs;

public class AttendeeDto
{
    public string UserId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? ImageUrl { get; set; }
    public bool IsHost { get; set; }
    public DateTime DateJoined { get; set; }
}

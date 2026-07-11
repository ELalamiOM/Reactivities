namespace Application.Activities.DTOs;

public class ActivityDto
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public DateTime Date { get; set; }
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public bool IsCancelled { get; set; }

    // host info
    public string HostId { get; set; } = "";
    public string HostDisplayName { get; set; } = "";

    // location props
    public string City { get; set; } = "";
    public string Venue { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public ICollection<UserProfileDto> Attendees { get; set; } = [];
}

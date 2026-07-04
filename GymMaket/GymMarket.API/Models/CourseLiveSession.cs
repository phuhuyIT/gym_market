namespace GymMarket.API.Models;

public partial class CourseLiveSession
{
    public string LiveSessionId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public string? MeetingUrl { get; set; }

    public string? RecordingUrl { get; set; }

    public string Status { get; set; } = CourseLiveSessionStatus.Draft;

    public bool AttendanceRequired { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public virtual Course? Course { get; set; }
}

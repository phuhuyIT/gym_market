namespace GymMarket.API.DTOs.CourseLiveSessions;

public class CourseLiveSessionDto
{
    public string LiveSessionId { get; set; } = string.Empty;

    public string CourseId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public string? MeetingUrl { get; set; }

    public string? RecordingUrl { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool AttendanceRequired { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }
}

public class UpsertCourseLiveSessionDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public string? MeetingUrl { get; set; }

    public string? RecordingUrl { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool AttendanceRequired { get; set; }
}

namespace GymMarket.API.DTOs.Announcements;

public class CourseAnnouncementDto
{
    public string AnnouncementId { get; set; } = string.Empty;
    public string CourseId { get; set; } = string.Empty;
    public string? CreatedByUserId { get; set; }
    public string CreatedByRole { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpsertCourseAnnouncementDto
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool IsPublished { get; set; }
}

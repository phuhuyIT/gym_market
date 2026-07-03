namespace GymMarket.API.Models;

public partial class CourseAnnouncement
{
    public string AnnouncementId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string? CreatedByUserId { get; set; }

    public string CreatedByRole { get; set; } = DiscussionAuthorRole.Trainer;

    public string CreatedByName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public bool IsPinned { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual AppUser? CreatedByUser { get; set; }
}

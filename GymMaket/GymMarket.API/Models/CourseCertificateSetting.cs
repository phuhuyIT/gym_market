namespace GymMarket.API.Models;

public partial class CourseCertificateSetting
{
    public string CourseId { get; set; } = null!;

    public bool IsEnabled { get; set; } = true;

    public string TemplateName { get; set; } = "Classic";

    public string CertificateTitle { get; set; } = "Certificate of Completion";

    public string BodyText { get; set; } = "Awarded for successfully completing this course.";

    public string AccentColor { get; set; } = "#007AFF";

    public decimal RequiredLecturePercent { get; set; } = 100m;

    public bool RequirePublishedQuizzes { get; set; } = true;

    public bool RequirePublishedAssignments { get; set; }

    public decimal RequiredAssignmentPercent { get; set; } = 0m;

    public decimal? MinimumFinalGradePercent { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }
}

namespace GymMarket.API.DTOs.Certificate;

public class CourseCertificateSettingDto
{
    public string CourseId { get; set; } = string.Empty;

    public string? CourseTitle { get; set; }

    public bool IsEnabled { get; set; } = true;

    public string TemplateName { get; set; } = "Classic";

    public string CertificateTitle { get; set; } = "Certificate of Completion";

    public string BodyText { get; set; } = "Awarded for successfully completing this course.";

    public string AccentColor { get; set; } = "#007AFF";

    public decimal RequiredLecturePercent { get; set; } = 100m;

    public bool RequirePublishedQuizzes { get; set; } = true;

    public bool RequirePublishedAssignments { get; set; }

    public decimal RequiredAssignmentPercent { get; set; }

    public decimal? MinimumFinalGradePercent { get; set; }

    public DateTime UpdatedAt { get; set; }
}

namespace GymMarket.API.DTOs.Certificate;

public class CourseCompletionStatusDto
{
    public string CourseId { get; set; } = string.Empty;

    public int TotalLectures { get; set; }

    public int CompletedLectures { get; set; }

    public bool LecturesCompleted { get; set; }

    public decimal LectureCompletionPercent { get; set; }

    public decimal RequiredLecturePercent { get; set; }

    public bool QuizRequired { get; set; }

    public bool QuizPassed { get; set; }

    public decimal? BestQuizScorePercent { get; set; }

    public bool AssignmentRequired { get; set; }

    public bool AssignmentPassed { get; set; } = true;

    public int TotalAssignments { get; set; }

    public int GradedAssignments { get; set; }

    public decimal? AssignmentAveragePercent { get; set; }

    public decimal RequiredAssignmentPercent { get; set; }

    public decimal? FinalGradePercent { get; set; }

    public decimal? MinimumFinalGradePercent { get; set; }

    public bool FinalGradePassed { get; set; } = true;

    public bool CertificatesEnabled { get; set; } = true;

    public CourseCertificateSettingDto? Setting { get; set; }

    public bool IsCompleted { get; set; }

    public CourseCertificateDto? Certificate { get; set; }
}

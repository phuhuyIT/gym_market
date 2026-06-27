namespace GymMarket.API.DTOs.Certificate;

public class CourseCompletionStatusDto
{
    public string CourseId { get; set; } = string.Empty;

    public int TotalLectures { get; set; }

    public int CompletedLectures { get; set; }

    public bool LecturesCompleted { get; set; }

    public bool QuizRequired { get; set; }

    public bool QuizPassed { get; set; }

    public decimal? BestQuizScorePercent { get; set; }

    public bool IsCompleted { get; set; }

    public CourseCertificateDto? Certificate { get; set; }
}

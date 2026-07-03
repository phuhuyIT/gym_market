namespace GymMarket.API.DTOs.Quiz;

public class QuizAttemptSummaryDto
{
    public string AttemptId { get; set; } = string.Empty;

    public string QuizId { get; set; } = string.Empty;

    public string StudentId { get; set; } = string.Empty;

    public string? StudentName { get; set; }

    public string? StudentEmail { get; set; }

    public int AttemptNumber { get; set; }

    public int Score { get; set; }

    public int TotalPoints { get; set; }

    public decimal ScorePercent { get; set; }

    public bool Passed { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool RequiresManualGrading { get; set; }

    public string? Feedback { get; set; }

    public DateTime SubmittedAt { get; set; }
}

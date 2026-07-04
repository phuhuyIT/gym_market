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

    public DateTime? StartedAt { get; set; }

    public bool HonorCodeAccepted { get; set; }

    public bool ProctoringReviewRequired { get; set; }

    public int FocusLostCount { get; set; }

    public int PasteEventCount { get; set; }

    public int FullscreenExitCount { get; set; }

    public int ProctoringEventCount { get; set; }

    public int SuspiciousActivityScore { get; set; }

    public string? ProctoringFlags { get; set; }

    public string? Feedback { get; set; }

    public DateTime SubmittedAt { get; set; }
}

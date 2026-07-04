using GymMarket.API;

namespace GymMarket.API.Models;

public partial class QuizAttempt
{
    public string AttemptId { get; set; } = null!;

    public string QuizId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public int AttemptNumber { get; set; } = 1;

    public int Score { get; set; }

    public int TotalPoints { get; set; }

    public decimal ScorePercent { get; set; }

    public bool Passed { get; set; }

    public DateTime SubmittedAt { get; set; }

    public string Status { get; set; } = QuizAttemptStatus.Submitted;

    public bool RequiresManualGrading { get; set; }

    public DateTime? StartedAt { get; set; }

    public string? QuestionOrderSnapshot { get; set; }

    public string? BrowserFingerprint { get; set; }

    public string? IpAddress { get; set; }

    public bool HonorCodeAccepted { get; set; }

    public bool ProctoringReviewRequired { get; set; }

    public int FocusLostCount { get; set; }

    public int PasteEventCount { get; set; }

    public int FullscreenExitCount { get; set; }

    public int ProctoringEventCount { get; set; }

    public int SuspiciousActivityScore { get; set; }

    public string? ProctoringFlags { get; set; }

    public DateTime? GradedAt { get; set; }

    public string? Feedback { get; set; }

    public virtual CourseQuiz? Quiz { get; set; }

    public virtual Student? Student { get; set; }

    public virtual ICollection<QuizAttemptAnswer> Answers { get; set; } = new List<QuizAttemptAnswer>();
}

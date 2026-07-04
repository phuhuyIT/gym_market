using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Quiz;

public class SubmitQuizAttemptDto
{
    public DateTime? StartedAt { get; set; }

    public bool HonorCodeAccepted { get; set; }

    public string? BrowserFingerprint { get; set; }

    public List<QuizProctoringSignalDto> ProctoringSignals { get; set; } = [];

    public List<SubmitQuizAnswerDto> Answers { get; set; } = [];
}

public class QuizProctoringSignalDto
{
    public string Type { get; set; } = string.Empty;

    public DateTime? OccurredAt { get; set; }

    public int Count { get; set; } = 1;
}

public class SubmitQuizAnswerDto
{
    [Required]
    public string QuestionId { get; set; } = string.Empty;

    public string? SelectedOptionId { get; set; }

    public List<string> SelectedOptionIds { get; set; } = [];

    public string? TextAnswer { get; set; }
}

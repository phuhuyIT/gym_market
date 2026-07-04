namespace GymMarket.API.DTOs.Quiz;

public class CourseQuizDto
{
    public string QuizId { get; set; } = string.Empty;

    public string CourseId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string ScopeType { get; set; } = string.Empty;

    public string? ModuleId { get; set; }

    public string? ModuleTitle { get; set; }

    public string? LectureId { get; set; }

    public string? LectureTitle { get; set; }

    public int PassingScorePercent { get; set; }

    public int? TimeLimitMinutes { get; set; }

    public int? MaxAttempts { get; set; }

    public int AttemptsUsed { get; set; }

    public int? AttemptsRemaining { get; set; }

    public bool ShuffleQuestions { get; set; }

    public bool ShuffleOptions { get; set; }

    public bool ShowCorrectAnswers { get; set; }

    public bool RequireHonorCode { get; set; }

    public bool TrackProctoringSignals { get; set; }

    public DateTime? AvailableFrom { get; set; }

    public DateTime? AvailableUntil { get; set; }

    public bool IsPublished { get; set; }

    public List<QuizQuestionDto> Questions { get; set; } = [];

    public QuizAttemptSummaryDto? LatestAttempt { get; set; }

    public QuizAttemptSummaryDto? BestAttempt { get; set; }
}

public class QuizQuestionDto
{
    public string QuestionId { get; set; } = string.Empty;

    public string Prompt { get; set; } = string.Empty;

    public string QuestionType { get; set; } = string.Empty;

    public int Order { get; set; }

    public int Points { get; set; }

    public string? Explanation { get; set; }

    public string? QuestionBank { get; set; }

    public List<QuizOptionDto> Options { get; set; } = [];
}

public class QuizOptionDto
{
    public string OptionId { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
}

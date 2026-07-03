namespace GymMarket.API.DTOs.Quiz;

public class TrainerCourseQuizDto
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

    public bool ShuffleQuestions { get; set; }

    public bool ShowCorrectAnswers { get; set; }

    public DateTime? AvailableFrom { get; set; }

    public DateTime? AvailableUntil { get; set; }

    public bool IsPublished { get; set; }

    public List<TrainerQuizQuestionDto> Questions { get; set; } = [];
}

public class TrainerQuizQuestionDto
{
    public string QuestionId { get; set; } = string.Empty;

    public string Prompt { get; set; } = string.Empty;

    public string QuestionType { get; set; } = string.Empty;

    public int Order { get; set; }

    public int Points { get; set; }

    public string? Explanation { get; set; }

    public bool RequiresManualGrading { get; set; }

    public List<TrainerQuizOptionDto> Options { get; set; } = [];
}

public class TrainerQuizOptionDto
{
    public string OptionId { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}

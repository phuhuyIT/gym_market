namespace GymMarket.API.DTOs.Quiz;

public class CourseQuizDto
{
    public string QuizId { get; set; } = string.Empty;

    public string CourseId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int PassingScorePercent { get; set; }

    public bool IsPublished { get; set; }

    public List<QuizQuestionDto> Questions { get; set; } = [];

    public QuizAttemptSummaryDto? LatestAttempt { get; set; }

    public QuizAttemptSummaryDto? BestAttempt { get; set; }
}

public class QuizQuestionDto
{
    public string QuestionId { get; set; } = string.Empty;

    public string Prompt { get; set; } = string.Empty;

    public int Order { get; set; }

    public int Points { get; set; }

    public List<QuizOptionDto> Options { get; set; } = [];
}

public class QuizOptionDto
{
    public string OptionId { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
}

namespace GymMarket.API.DTOs.Quiz;

public class TrainerCourseQuizDto
{
    public string QuizId { get; set; } = string.Empty;

    public string CourseId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int PassingScorePercent { get; set; }

    public bool IsPublished { get; set; }

    public List<TrainerQuizQuestionDto> Questions { get; set; } = [];
}

public class TrainerQuizQuestionDto
{
    public string QuestionId { get; set; } = string.Empty;

    public string Prompt { get; set; } = string.Empty;

    public int Order { get; set; }

    public int Points { get; set; }

    public List<TrainerQuizOptionDto> Options { get; set; } = [];
}

public class TrainerQuizOptionDto
{
    public string OptionId { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}

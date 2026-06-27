namespace GymMarket.API.Models;

public partial class QuizAttemptAnswer
{
    public string AttemptAnswerId { get; set; } = null!;

    public string AttemptId { get; set; } = null!;

    public string QuestionId { get; set; } = null!;

    public string? SelectedOptionId { get; set; }

    public bool IsCorrect { get; set; }

    public int PointsAwarded { get; set; }

    public virtual QuizAttempt? Attempt { get; set; }

    public virtual QuizQuestion? Question { get; set; }

    public virtual QuizOption? SelectedOption { get; set; }
}

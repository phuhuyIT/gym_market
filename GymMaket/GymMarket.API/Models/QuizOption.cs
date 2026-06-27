namespace GymMarket.API.Models;

public partial class QuizOption
{
    public string OptionId { get; set; } = null!;

    public string QuestionId { get; set; } = null!;

    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public virtual QuizQuestion? Question { get; set; }

    public virtual ICollection<QuizAttemptAnswer> AttemptAnswers { get; set; } = new List<QuizAttemptAnswer>();
}

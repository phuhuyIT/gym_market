using GymMarket.API;

namespace GymMarket.API.Models;

public partial class QuizQuestion
{
    public string QuestionId { get; set; } = null!;

    public string QuizId { get; set; } = null!;

    public string Prompt { get; set; } = string.Empty;

    public string QuestionType { get; set; } = QuizQuestionType.SingleChoice;

    public int Order { get; set; }

    public int Points { get; set; } = 1;

    public string? Explanation { get; set; }

    public bool RequiresManualGrading { get; set; }

    public virtual CourseQuiz? Quiz { get; set; }

    public virtual ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();

    public virtual ICollection<QuizAttemptAnswer> AttemptAnswers { get; set; } = new List<QuizAttemptAnswer>();
}

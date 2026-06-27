namespace GymMarket.API.Models;

public partial class CourseQuiz
{
    public string QuizId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public int PassingScorePercent { get; set; } = 70;

    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();

    public virtual ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}

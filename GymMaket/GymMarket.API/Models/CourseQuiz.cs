using GymMarket.API;

namespace GymMarket.API.Models;

public partial class CourseQuiz
{
    public string QuizId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string ScopeType { get; set; } = AssessmentScopeType.Course;

    public string? ModuleId { get; set; }

    public string? LectureId { get; set; }

    public int PassingScorePercent { get; set; } = 70;

    public int? TimeLimitMinutes { get; set; }

    public int? MaxAttempts { get; set; }

    public bool ShuffleQuestions { get; set; }

    public bool ShowCorrectAnswers { get; set; }

    public DateTime? AvailableFrom { get; set; }

    public DateTime? AvailableUntil { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual CourseModule? Module { get; set; }

    public virtual Lecture? Lecture { get; set; }

    public virtual ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();

    public virtual ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}

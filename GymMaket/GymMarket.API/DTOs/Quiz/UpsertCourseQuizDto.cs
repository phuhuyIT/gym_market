using System.ComponentModel.DataAnnotations;
using GymMarket.API;

namespace GymMarket.API.DTOs.Quiz;

public class UpsertCourseQuizDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public string ScopeType { get; set; } = AssessmentScopeType.Course;

    public string? ModuleId { get; set; }

    public string? LectureId { get; set; }

    [Range(0, 100)]
    public int PassingScorePercent { get; set; } = 70;

    [Range(1, 1440)]
    public int? TimeLimitMinutes { get; set; }

    [Range(1, 100)]
    public int? MaxAttempts { get; set; }

    public bool ShuffleQuestions { get; set; }

    public bool ShuffleOptions { get; set; }

    public bool ShowCorrectAnswers { get; set; }

    public bool RequireHonorCode { get; set; }

    public bool TrackProctoringSignals { get; set; }

    public DateTime? AvailableFrom { get; set; }

    public DateTime? AvailableUntil { get; set; }

    public bool IsPublished { get; set; }

    public List<UpsertQuizQuestionDto> Questions { get; set; } = [];
}

public class UpsertQuizQuestionDto
{
    public string? QuestionId { get; set; }

    [Required]
    public string Prompt { get; set; } = string.Empty;

    public string QuestionType { get; set; } = QuizQuestionType.SingleChoice;

    [Range(1, int.MaxValue)]
    public int Order { get; set; }

    [Range(1, 100)]
    public int Points { get; set; } = 1;

    public string? Explanation { get; set; }

    public string? QuestionBank { get; set; }

    public List<UpsertQuizOptionDto> Options { get; set; } = [];
}

public class UpsertQuizOptionDto
{
    public string? OptionId { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}

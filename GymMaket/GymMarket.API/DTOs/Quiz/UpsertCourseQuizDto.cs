using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Quiz;

public class UpsertCourseQuizDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(0, 100)]
    public int PassingScorePercent { get; set; } = 70;

    public bool IsPublished { get; set; }

    public List<UpsertQuizQuestionDto> Questions { get; set; } = [];
}

public class UpsertQuizQuestionDto
{
    public string? QuestionId { get; set; }

    [Required]
    public string Prompt { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Order { get; set; }

    [Range(1, 100)]
    public int Points { get; set; } = 1;

    public List<UpsertQuizOptionDto> Options { get; set; } = [];
}

public class UpsertQuizOptionDto
{
    public string? OptionId { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}

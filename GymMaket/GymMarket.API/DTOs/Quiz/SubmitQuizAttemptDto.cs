using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Quiz;

public class SubmitQuizAttemptDto
{
    public List<SubmitQuizAnswerDto> Answers { get; set; } = [];
}

public class SubmitQuizAnswerDto
{
    [Required]
    public string QuestionId { get; set; } = string.Empty;

    public string? SelectedOptionId { get; set; }
}

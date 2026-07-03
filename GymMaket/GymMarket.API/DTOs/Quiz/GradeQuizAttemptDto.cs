using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Quiz;

public class GradeQuizAttemptDto
{
    [Range(0, int.MaxValue)]
    public int Score { get; set; }

    [MaxLength(2000)]
    public string? Feedback { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.CourseRating
{
    public class CourseRatingUpdateDto
    {
        [Required(ErrorMessage = "RatingId is required.")]
        public string RatingId { get; set; } = null!;
        [Required(ErrorMessage = "CourseId is required.")]
        public string? CourseId { get; set; }
        [Required(ErrorMessage = "StudentId is required.")]
        public string? StudentId { get; set; }
        [Required(ErrorMessage = "RatingValue is required.")]
        public decimal? RatingValue { get; set; }
        [Required(ErrorMessage = "ReviewComment is required.")]
        public string? ReviewComment { get; set; }

    }
}

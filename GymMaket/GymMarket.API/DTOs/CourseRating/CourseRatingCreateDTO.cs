using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.CourseRating
{
    public class CourseRatingCreateDTO
    {
        [Required(ErrorMessage = "RatingId is required.")]
        public string RatingId { get; set; } = null!;

        [Required(ErrorMessage = "CourseId is required.")]
        public string? CourseId { get; set; }

        [Required(ErrorMessage = "StudentId is required.")]
        public string? StudentId { get; set; }

        [Required(ErrorMessage = "RatingValue is required.")]
        [Range(0, 5, ErrorMessage = "RatingValue must be between 0 and 5.")]
        public decimal? RatingValue { get; set; }

        [StringLength(500, ErrorMessage = "ReviewComment cannot exceed 500 characters.")]
        public string? ReviewComment { get; set; }
    }
}

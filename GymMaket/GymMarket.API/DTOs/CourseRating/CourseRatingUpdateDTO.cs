using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.CourseRating
{
    public class CourseRatingUpdateDTO
    {
        [Required(ErrorMessage = "RatingValue is required.")]
        [Range(0, 5, ErrorMessage = "RatingValue must be between 0 and 5.")]
        public decimal? RatingValue { get; set; }

        [StringLength(500, ErrorMessage = "ReviewComment cannot exceed 500 characters.")]
        public string? ReviewComment { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Course
{
    public class CourseCreateDTO
    {
        [Required(ErrorMessage = "Course ID is required")]
        [StringLength(100, ErrorMessage = "Course ID cannot exceed 50 characters")]
        public string CourseId { get; set; } = null!;

        [StringLength(100)]
        public string? TrainerId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public decimal? Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Additional price must be a positive number")]
        public decimal? AdditionalPrice { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be a positive number(weeks)")]
        public int? Duration { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Maximum participants must be a positive number")]
        public int? MaxParticipants { get; set; }
    }
}

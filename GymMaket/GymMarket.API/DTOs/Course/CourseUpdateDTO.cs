using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Course
{
    public class CourseUpdateDTO
    {
        public string CourseId { get; set; } = string.Empty;
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? AdditionalPrice { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(1, int.MaxValue)]
        public int? Duration { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxParticipants { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

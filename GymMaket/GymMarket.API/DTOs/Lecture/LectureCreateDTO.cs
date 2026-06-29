using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Lecture
{
    public class LectureCreateDTO
    {
        [Required(ErrorMessage = "LectureId is required.")]
        public string LectureId { get; set; } = null!;

        public string? CourseId { get; set; }

        public string? ModuleId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public string? ActivityType { get; set; }

        public int? Order { get; set; }

        public int? Duration { get; set; }

        public string? PrerequisiteLectureId { get; set; }

        public int? UnlockAfterDays { get; set; }

        public DateTime? AvailableFrom { get; set; }

        public DateTime? AvailableUntil { get; set; }

        public bool IsPreview { get; set; }

        public bool IsPublished { get; set; } = true;
    }
}

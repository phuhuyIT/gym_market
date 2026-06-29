using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.CourseModule
{
    public class CourseModuleUpdateDto
    {
        [Required(ErrorMessage = "ModuleId is required.")]
        public string ModuleId { get; set; } = null!;

        public string? CourseId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string Title { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public int? Order { get; set; }

        public string? PrerequisiteModuleId { get; set; }

        public int? UnlockAfterDays { get; set; }

        public DateTime? AvailableFrom { get; set; }

        public DateTime? AvailableUntil { get; set; }

        public bool IsPublished { get; set; } = true;
    }
}

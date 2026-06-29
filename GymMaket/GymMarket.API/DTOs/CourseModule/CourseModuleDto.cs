namespace GymMarket.API.DTOs.CourseModule
{
    public class CourseModuleDto
    {
        public string ModuleId { get; set; } = null!;
        public string? CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Order { get; set; }
        public string? PrerequisiteModuleId { get; set; }
        public int? UnlockAfterDays { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableUntil { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

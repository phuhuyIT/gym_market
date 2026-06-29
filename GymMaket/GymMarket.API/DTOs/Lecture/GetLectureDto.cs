namespace GymMarket.API.DTOs.Lecture
{
    public class GetLectureDto
    {
        public string LectureId { get; set; } = null!;
        public string? CourseId { get; set; }
        public string? ModuleId { get; set; }
        public string? ModuleTitle { get; set; }
        public int? ModuleOrder { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ActivityType { get; set; }
        public int? Order { get; set; }
        public int? Duration { get; set; }
        public string? PrerequisiteLectureId { get; set; }
        public int? UnlockAfterDays { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableUntil { get; set; }
        public bool IsPreview { get; set; }
        public bool IsPublished { get; set; }
        public bool IsLocked { get; set; }
        public string? LockReason { get; set; }
        public DateTime? UnlocksAt { get; set; }
    }
}

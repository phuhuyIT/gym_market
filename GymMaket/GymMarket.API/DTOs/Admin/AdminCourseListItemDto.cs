using GymMarket.API;

namespace GymMarket.API.DTOs.Admin
{
    public class AdminCourseListItemDto
    {
        public string CourseId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Category { get; set; }
        public decimal? Price { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = CourseStatus.PendingReview;
        public string? TrainerId { get; set; }
        public string? TrainerName { get; set; }
        public string? TrainerEmail { get; set; }
        public string? TrainerApprovalStatus { get; set; }
    }
}

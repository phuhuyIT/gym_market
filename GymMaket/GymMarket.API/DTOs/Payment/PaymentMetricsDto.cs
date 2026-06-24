using GymMarket.API;

namespace GymMarket.API.DTOs.Payment
{
    public class PaymentMetricsDto
    {
        public decimal TotalPaidRevenue { get; set; }
        public decimal PendingAmount { get; set; }
        public int PaidCount { get; set; }
        public int PendingCount { get; set; }
        public int CanceledCount { get; set; }
        public int ExpiredCount { get; set; }
        public int UniquePaidStudentCount { get; set; }
        public List<CourseRevenueDto> RevenueByCourse { get; set; } = [];
        public List<RecentPaidPaymentDto> RecentPaidPayments { get; set; } = [];
    }

    public class CourseRevenueDto
    {
        public string CourseId { get; set; } = null!;
        public string? CourseTitle { get; set; }
        public decimal PaidRevenue { get; set; }
        public int PaidCount { get; set; }
    }

    public class RecentPaidPaymentDto
    {
        public string PaymentId { get; set; } = null!;
        public string? CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public string? StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? UserId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentStatus { get; set; } = GymMarket.API.PaymentStatus.Paid;
        public DateTime? PaidAt { get; set; }
    }
}

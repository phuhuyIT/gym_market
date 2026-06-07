namespace GymMarket.API.DTOs.Payment
{
    public class GetPaymentDto
    {
        public string PaymentId { get; set; } = null!;
        public string? CourseId { get; set; }
        public string? StudentId { get; set; }
        public decimal? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentType { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }

        // Flattened from the related Student so the client can show the student's name.
        public string? FullName { get; set; }
    }
}

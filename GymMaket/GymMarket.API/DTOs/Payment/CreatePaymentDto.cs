namespace GymMarket.API.DTOs.Payment
{
    public class CreatePaymentDto
    {
        public string CourseId { get; set; } = null!;
        public string StudentId { get; set; } = null!;
    }
}

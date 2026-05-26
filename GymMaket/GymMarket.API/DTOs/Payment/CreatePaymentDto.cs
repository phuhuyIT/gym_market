using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "CourseId is required.")]
        public string CourseId { get; set; } = null!;

        [Required(ErrorMessage = "StudentId is required.")]
        public string StudentId { get; set; } = null!;
    }
}

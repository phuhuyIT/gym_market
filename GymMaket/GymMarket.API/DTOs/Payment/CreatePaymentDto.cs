using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Payment
{
    // The paying student is always the authenticated caller (studentId JWT claim),
    // so the DTO carries no student id.
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "CourseId is required.")]
        public string CourseId { get; set; } = null!;
    }
}

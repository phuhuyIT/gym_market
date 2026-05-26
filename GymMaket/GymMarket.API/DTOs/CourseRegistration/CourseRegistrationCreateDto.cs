using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.CourseRegistration
{
    public class CourseRegistrationCreateDto
    {
        [Required(ErrorMessage = "CourseId is required.")]
        public string CourseId { get; set; } = null!;

        [Required(ErrorMessage = "StudentId is required.")]
        public string StudentId { get; set; } = null!;

        public string? RegistrationType { get; set; }

        public string? Mode { get; set; }

        public string? Status { get; set; }

        public string? PaymentStatus { get; set; }

        public decimal? InitialPayment { get; set; }

        public decimal? AdditionalFeaturesPayment { get; set; }

        public string? ContractAgreement { get; set; }
    }
}

namespace GymMarket.API.DTOs.CourseRegistration
{
    public class CourseRegistrationUpdateDto
    {
        public string? CourseId { get; set; }

        public string? StudentId { get; set; }

        public string? RegistrationType { get; set; }

        public string? Mode { get; set; }

        public string? Status { get; set; }

        public string? PaymentStatus { get; set; }

        public decimal? InitialPayment { get; set; }

        public decimal? AdditionalFeaturesPayment { get; set; }

        public string? ContractAgreement { get; set; }
    }
}

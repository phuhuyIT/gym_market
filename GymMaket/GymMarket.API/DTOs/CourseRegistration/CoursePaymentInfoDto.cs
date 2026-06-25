namespace GymMarket.API.DTOs.CourseRegistration
{
    // Everything the student's payment screen needs to render a bank-transfer QR
    // for a course they have registered for. Scoped to the caller's own registration.
    public class CoursePaymentInfoDto
    {
        public string? PaymentId { get; set; }
        public string CourseId { get; set; } = string.Empty;
        public string? CourseTitle { get; set; }
        public decimal Amount { get; set; }
        public decimal CourseAmount { get; set; }
        public decimal OptionsAmount { get; set; }
        public List<CoursePaymentOptionDto> Options { get; set; } = [];

        // Normalized payment status: Pending / Paid / Canceled / Not Started.
        public string? Status { get; set; }

        // Transfer memo the student must include so the trainer can match the deposit.
        public string? Reference { get; set; }

        // Receiving account (the course trainer's). BankConfigured is false when the
        // trainer has not set up an account yet, so the client can show a friendly notice.
        public string? BankBin { get; set; }
        public string? BankAccountNo { get; set; }
        public string? BankAccountName { get; set; }
        public string? TrainerName { get; set; }
        public bool BankConfigured { get; set; }
    }

    public class CoursePaymentOptionDto
    {
        public string OptionId { get; set; } = string.Empty;
        public string? OptionName { get; set; }
        public decimal Price { get; set; }
    }
}

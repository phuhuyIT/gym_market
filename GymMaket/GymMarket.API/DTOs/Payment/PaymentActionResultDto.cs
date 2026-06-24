namespace GymMarket.API.DTOs.Payment
{
    public class PaymentActionResultDto
    {
        public bool Succeeded { get; set; }
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }
        public Models.Payment? Payment { get; set; }

        public static PaymentActionResultDto Success(Models.Payment payment) => new()
        {
            Succeeded = true,
            Payment = payment
        };

        public static PaymentActionResultDto Failure(string errorCode, string message, Models.Payment? payment = null) => new()
        {
            Succeeded = false,
            ErrorCode = errorCode,
            Message = message,
            Payment = payment
        };
    }
}

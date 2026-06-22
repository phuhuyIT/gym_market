namespace GymMarket.API.DTOs.Momo
{
    public class MomoCreatePaymentResultDto
    {
        public bool Success { get; set; }
        public string? ErrorCode { get; set; }
        public MomoCreatePaymentResponseModel? Payment { get; set; }

        public static MomoCreatePaymentResultDto Ok(MomoCreatePaymentResponseModel payment) =>
            new()
            {
                Success = true,
                Payment = payment
            };

        public static MomoCreatePaymentResultDto Fail(string errorCode) =>
            new()
            {
                Success = false,
                ErrorCode = errorCode
            };
    }
}

namespace GymMarket.API.DTOs.Payment
{
    public class CancelPayment
    {
        public string PaymentId { get; set; } = string.Empty;
        public string Note { get; set;} = string.Empty;
    }
}

namespace GymMarket.API.DTOs.Momo
{
    public class MomoCallbackDto
    {
        public string PartnerCode { get; set; } = null!;
        public string OrderId { get; set; } = null!;
        public string RequestId { get; set; } = null!;
        public string Amount { get; set; } = null!;
        public string OrderInfo { get; set; } = null!;
        public string OrderType { get; set; } = null!;
        public string TransId { get; set; } = null!;
        public int ResultCode { get; set; }
        public string Message { get; set; } = null!;
        public string PayType { get; set; } = null!;
        public string ResponseTime { get; set; } = null!;
        public string ExtraData { get; set; } = null!;
        public string Signature { get; set; } = null!;
    }
}

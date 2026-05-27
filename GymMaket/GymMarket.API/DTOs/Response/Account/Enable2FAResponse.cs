namespace GymMarket.API.DTOs.Response.Account
{
    public class Enable2FAResponse : ApiResponse
    {
        public string? SharedKey { get; set; }
        public string? QrCodeUri { get; set; }
    }
}

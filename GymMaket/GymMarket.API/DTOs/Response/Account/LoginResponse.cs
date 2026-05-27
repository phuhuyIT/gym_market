namespace GymMarket.API.DTOs.Response.Account
{
    public class LoginResponse : ApiResponse
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}

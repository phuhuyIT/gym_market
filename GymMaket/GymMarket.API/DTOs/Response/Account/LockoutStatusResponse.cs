namespace GymMarket.API.DTOs.Response.Account
{
    public class LockoutStatusResponse : ApiResponse
    {
        public bool IsLockedOut { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
    }
}

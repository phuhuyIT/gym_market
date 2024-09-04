namespace GymMarket.API.DTOs.Response
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; } = [];
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}

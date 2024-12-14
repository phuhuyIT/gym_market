namespace GymMarket.API.DTOs.FileMinIO
{
    public class GetFileDto
    {
        public string ObjectId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string TypeFile { get; set; } = string.Empty;
    }
}

namespace GymMarket.API.DTOs.FileMinIO
{
    public class FileAddResult
    {
        public string ObjectId { get; set; } = string.Empty;
        public string Url {  get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
    }
}

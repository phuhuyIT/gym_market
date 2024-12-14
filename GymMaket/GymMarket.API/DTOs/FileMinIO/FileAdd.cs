namespace GymMarket.API.DTOs.FileMinIO
{
    public class FileAdd
    {
        public List<IFormFile> Images { get; set; } = [];
        public List<IFormFile> Videos { get; set; } = [];
        public string CourseId { get; set; } = string.Empty;
    }
}

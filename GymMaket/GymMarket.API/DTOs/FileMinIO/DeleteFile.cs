namespace GymMarket.API.DTOs.FileMinIO
{
    public class DeleteFile
    {
        public string ObjectName { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
    }
}

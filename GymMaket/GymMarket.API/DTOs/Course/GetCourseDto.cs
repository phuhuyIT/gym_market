using GymMarket.API.DTOs.FileMinIO;

namespace GymMarket.API.DTOs.Course
{
    public class GetCourseDto
    {
        public string CourseId { get; set; } = null!;

        public string? TrainerId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Type { get; set; }

        public string? Category { get; set; }

        public decimal? Price { get; set; }

        public decimal? AdditionalPrice { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? Duration { get; set; }

        public int? MaxParticipants { get; set; }

        public string? StatusPayment { get; set; } = string.Empty;

        public decimal? Rating { get; set; }

        public List<GetFileDto> GetFileDtos { get; set; } = [];
    }
}

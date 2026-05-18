namespace GymMarket.API.DTOs.Lecture
{
    public class GetLectureDto
    {
        public string LectureId { get; set; } = null!;
        public string? CourseId { get; set; }
        public string? Title { get; set; }
        public int? Order { get; set; }
        public int? Duration { get; set; }
    }
}

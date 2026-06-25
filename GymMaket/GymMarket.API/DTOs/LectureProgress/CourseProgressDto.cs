namespace GymMarket.API.DTOs.LectureProgress
{
    public class CourseProgressDto
    {
        public string CourseId { get; set; } = null!;
        public int TotalLectures { get; set; }
        public int CompletedLectures { get; set; }
        public int ProgressPercent { get; set; }
        public List<string> CompletedLectureIds { get; set; } = [];
    }
}

using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Lecture
{
    public class LectureCreateDTO
    {
        [Required(ErrorMessage = "LectureId is required.")]
        public string LectureId { get; set; } = null!;

        public string? CourseId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string? Title { get; set; }

        public int? Order { get; set; }

        public int? Duration { get; set; }
    }
}

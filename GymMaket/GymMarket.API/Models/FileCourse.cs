using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymMarket.API.Models
{
    [Table("FileCourses")]
    public class FileCourse
    {
        [Key]
        public int Id { get; set; } 

        public string CourseId { get; set; } = string.Empty;
        public string ObjectId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string TypeFile { get; set; } = string.Empty;
    }
}

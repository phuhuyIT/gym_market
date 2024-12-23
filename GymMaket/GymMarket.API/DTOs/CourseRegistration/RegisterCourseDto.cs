using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.DTOs.CourseRegistration
{
    public class RegisterCourseDto
    {
        public string CourseId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
    }
}

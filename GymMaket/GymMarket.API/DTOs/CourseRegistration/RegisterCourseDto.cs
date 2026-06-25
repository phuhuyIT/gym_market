namespace GymMarket.API.DTOs.CourseRegistration
{
    // The registering student is always the authenticated caller (studentId JWT
    // claim), so the DTO carries no student id.
    public class RegisterCourseDto
    {
        public string CourseId { get; set; } = string.Empty;
        public List<string> OptionIds { get; set; } = [];
    }
}

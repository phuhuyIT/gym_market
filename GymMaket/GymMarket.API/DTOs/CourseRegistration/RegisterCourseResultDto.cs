using CourseRegistrationModel = GymMarket.API.Models.CourseRegistration;

namespace GymMarket.API.DTOs.CourseRegistration
{
    public class RegisterCourseResultDto
    {
        public bool Success { get; set; }
        public string? ErrorCode { get; set; }
        public CourseRegistrationModel? Registration { get; set; }

        public static RegisterCourseResultDto Ok(CourseRegistrationModel registration) =>
            new()
            {
                Success = true,
                Registration = registration
            };

        public static RegisterCourseResultDto Fail(string errorCode) =>
            new()
            {
                Success = false,
                ErrorCode = errorCode
            };
    }
}

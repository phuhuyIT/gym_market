using GymMarket.API.DTOs.Student;

namespace GymMarket.API.DTOs.Response.Student
{
    public class GetStudentProfileResponse : ApiResponse
    {
        public StudentProfileDto? StudentProfile { get; set; }
    }
}

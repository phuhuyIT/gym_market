using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseRegistrationRepository : IGenericRepository<CourseRegistration, string>
    {
        Task<RegisterCourseResultDto> RegisterCourseAsync(RegisterCourseDto dto, string studentId);
        Task<bool> InitializePaymentAsync(string registrationId, decimal initialPayment, string studentId);
        Task<bool> CancelRegistrationAsync(string registrationId, string studentId);
        Task<List<GetCourseDto>> GetCourseRegistrations(string studentId);
        Task<CoursePaymentInfoDto?> GetCoursePaymentInfo(string studentId, string courseId);

        // The student claims they have transferred the money. Notifies the trainer to
        // verify and approve; does NOT mark the payment paid. Returns current info, or
        // null when the caller has no registration for the course.
        Task<CoursePaymentInfoDto?> ConfirmPaymentByStudent(string studentId, string courseId);
    }
}

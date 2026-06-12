using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseRegistrationRepository : IGenericRepository<CourseRegistration, string>
    {
        Task<CourseRegistration?> RegisterCourseAsync(RegisterCourseDto dto, string studentId);
        Task<bool> InitializePaymentAsync(string registrationId, decimal initialPayment, string studentId);
        Task<bool> CancelRegistrationAsync(string registrationId, string studentId);
        Task<List<GetCourseDto>> GetCourseRegistrations(string studentId);
    }
}

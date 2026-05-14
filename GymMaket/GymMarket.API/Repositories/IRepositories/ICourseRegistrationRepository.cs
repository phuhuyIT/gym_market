using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseRegistrationRepository : IGenericRepository<CourseRegistration, string>
    {
        Task<CourseRegistration?> RegisterCourseAsync(RegisterCourseDto dto);
        Task<bool> InitializePaymentAsync(string registrationId, decimal initialPayment);
        Task<bool> CancelRegistrationAsync(string registrationId);
        Task<List<GetCourseDto>> GetCourseRegistrations(string studentId);
    }
}

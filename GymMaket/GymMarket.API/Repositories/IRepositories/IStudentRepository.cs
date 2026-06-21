using GymMarket.API.DTOs.Response.Student;
using GymMarket.API.DTOs.Response;
using GymMarket.API.DTOs.Student;
using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IStudentRepository : IGenericRepository<Student, string>
    {
        Task<GetStudentProfileResponse> GetStudentProfileByUserId(string userId);
        Task<PagedResult<StudentSearchDto>> SearchStudents(
            int pageIndex = 1,
            int pageSize = Defaults.PageSize,
            string? search = null,
            string? trainerId = null,
            bool includeAllStudents = false);
    }
}

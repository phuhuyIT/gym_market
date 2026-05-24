using GymMarket.API.DTOs.Response.Student;
using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IStudentRepository : IGenericRepository<Student, string>
    {
        Task<GetStudentProfileResponse> GetStudentProfileByUserId(string userId);
    }
}

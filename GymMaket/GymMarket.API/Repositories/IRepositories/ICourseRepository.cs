using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseRepository : IGenericRepository<Course, string>
    {
        Task<List<Course>> GetCoursesByTrainer(string trainerId);
    }
}

using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseRepository:IGenericRepository<Course>
    {
        Task<List<Course>> GetTopRatedCoursesAsync(int topRate);
        Task<List<Course>> GetNewestCoursesAsync(int topRate);
    }
}

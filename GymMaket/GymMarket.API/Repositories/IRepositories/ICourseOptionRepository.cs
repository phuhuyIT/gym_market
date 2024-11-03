using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseOptionRepository: IGenericRepository<CourseOption>
    {
        Task<IEnumerable<CourseOption>> GetByCourseIdAsync(string courseId);
    }
}

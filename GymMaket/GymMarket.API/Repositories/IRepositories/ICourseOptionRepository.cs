using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseOptionRepository: IGenericRepository<CourseOption, string>
    {
        Task<IEnumerable<CourseOption>> GetByCourseIdAsync(string courseId);
    }
}

using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseRatingRepository : IGenericRepository<CourseRating, string>
    {
        Task<IEnumerable<CourseRating>> GetByCourseIdAsync(string courseId);
    }
}

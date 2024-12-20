using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseRepository
    {
        Task<ICollection<Course>> GetCoursesOfTrainer(string trainerId);
        Task<IEnumerable<Course>> SearchAndFilterCoursesAsync(string? keyword, string? decription,
            decimal? minPrice, decimal? maxPrice, int? minDuration, int? maxDuration, double? minRating, string? category);
    }
}

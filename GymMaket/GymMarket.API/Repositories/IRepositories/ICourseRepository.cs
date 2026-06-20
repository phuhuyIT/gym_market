using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseRepository
    {
        Task<ICollection<GetCourseDto>> GetCoursesOfTrainer(string trainerId);
        Task<ApiResponse> UpdateCourse(CourseUpdateDTO courseUpdateDTO);
        Task<GetCourseDto?> GetCourse(string courseId);
        Task<PagedResult<GetCourseDto>> GetCourses(int pageIndex = 1, int pageSize = Defaults.PageSize, string? searchString = null, string? category = null);
        Task<IEnumerable<Course>> SearchAndFilterCoursesAsync(string? keyword, string? decription,
    decimal? minPrice, decimal? maxPrice, int? minDuration, int? maxDuration, double? minRating, string? category);
    }
}

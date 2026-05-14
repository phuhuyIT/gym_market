using GymMarket.API.DTOs.CourseRating;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface ICourseRatingRepository
    {
        Task<IEnumerable<CourseRating>> GetRatingsByCourseId(string courseId);
        Task<ApiResponse> AddRating(CourseRatingCreateDto dto);
    }
}

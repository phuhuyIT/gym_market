using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.CourseRating;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class CourseRatingRepository : ICourseRatingRepository
    {
        private readonly GymMarketContext _context;
        private readonly IMapper _mapper;

        public CourseRatingRepository(GymMarketContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse> AddRating(CourseRatingCreateDto courseRatingCreateDTO)
        {
            var courseRating = new CourseRating
            {
                CourseId = courseRatingCreateDTO.CourseId,
                RatingId = courseRatingCreateDTO.RatingId,
                RatingValue = courseRatingCreateDTO.RatingValue,
                ReviewComment = courseRatingCreateDTO.ReviewComment,
                StudentId = courseRatingCreateDTO.StudentId,
            };

            var ratings = await _context.CourseRatings
                   .AsNoTrackingWithIdentityResolution().Where(c => c.CourseId == courseRating.CourseId)
                   .Select(c => c.RatingValue)
                   .ToListAsync();

            var averageRating = (ratings.Sum() + courseRating.RatingValue) / (ratings.Count() + 1);

            var course = await _context.Courses
                .AsNoTrackingWithIdentityResolution()
                .Where(c => c.CourseId == courseRating.CourseId)
                .FirstOrDefaultAsync();
            if (course != null)
            {
                course.Rating = averageRating;
                _context.Courses.Update(course);
            }
            _context.CourseRatings.Add(courseRating);
            var r = await _context.SaveChangesAsync();
            if(r > 0)
            {
                return new ApiResponse { Message = "SUCCESS", StatusCode = 200, Success = true };
            }
            return new ApiResponse { Errors = ["ADD_RATING_FAILED"], StatusCode = 400, Success = false };
        }

        public async Task<IEnumerable<CourseRating>> GetRatingsByCourseId(string courseId)
        {
            var ratings = await _context.CourseRatings.AsNoTrackingWithIdentityResolution().Where(c => c.CourseId == courseId).ToListAsync();
            return ratings;
        }
    }
}

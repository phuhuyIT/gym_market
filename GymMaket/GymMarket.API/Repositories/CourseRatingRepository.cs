using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.CourseRating;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class CourseRatingRepository
    {
        private readonly GymMarketContext _context;
        private readonly IMapper mapper;

        public CourseRatingRepository(GymMarketContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        public async Task<ApiResponse> AddCourseRating(CourseRatingCreateDTO courseRatingCreateDTO)
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

            var verageRating = (ratings.Sum() + courseRating.RatingValue) / (ratings.Count() + 1);

            var course = await _context.Courses
                .AsNoTrackingWithIdentityResolution()
                .Where(c => c.CourseId == courseRating.CourseId)
                .FirstOrDefaultAsync();
            if (course != null)
            {
                course.Rating = verageRating;
                _context.Courses.Update(course);
            }
            _context.CourseRatings.Add(courseRating);
            var r = await _context.SaveChangesAsync();
            if(r > 0)
            {
                return new DTOs.Response.ApiResponse { Message = "Thêm thành công", StatusCode = 200, Success = true };
            }
            return new DTOs.Response.ApiResponse { Message = "Thêm thất bại. Vui lòng thử lại.", StatusCode = 400, Success = false };
        }

        public async Task<List<CourseRating>> GetCourseRatings(string courseId)
        {
            var ratings = await _context.CourseRatings.AsNoTrackingWithIdentityResolution().Where(c => c.CourseId == courseId).ToListAsync();
            return ratings;
        }
    }
}

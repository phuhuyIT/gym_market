using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        private readonly GymMarketContext _context;
        public CourseRepository(GymMarketContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetNewestCoursesAsync(int topCourse)
        {
            var coursesNewest = await _context.Courses
        .Where(c => c.StartDate.HasValue)
        .OrderByDescending(c => c.StartDate)
        .Take(topCourse)
        .ToListAsync();

            return coursesNewest;
        }

        public async Task<List<Course>> GetTopRatedCoursesAsync(int topRate)
        {
            var coursesTopRated = await _context.Courses
         .Where(c => c.Rating.HasValue)
         .OrderByDescending(c => c.Rating)
         .Take(topRate)
         .ToListAsync();
            return coursesTopRated;
        }
    }
}

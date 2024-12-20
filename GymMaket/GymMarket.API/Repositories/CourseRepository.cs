using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class CourseRepository : GenericRepository<Course, string>, ICourseRepository
    {
        private readonly GymMarketContext _context;
        public CourseRepository(GymMarketContext context, IMapper mapper) : base(context,mapper)
        {
            _context = context;
        }

        //override getall method
        public override async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.CourseRatings)
                .Include(c => c.Lectures)
                .Include(c => c.Trainer)
                .ToListAsync();
        }
        //override getbyid method
        public override async Task<Course> GetByIdAsync(string id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseRatings)
                .Include(c => c.Lectures)
                .Include(c => c.Trainer)
                .FirstOrDefaultAsync(c => c.CourseId == id);
            return course;
        }

         
        async Task<ICollection<Course>> ICourseRepository.GetCoursesOfTrainer(string trainerId)
        {
            var courses = await _context.Courses.AsNoTrackingWithIdentityResolution()
                .Where(c => c.TrainerId == trainerId)
                .ToListAsync();

            return courses;
        }
        public async Task<IEnumerable<Course>> SearchAndFilterCoursesAsync(string? keyword, string? decription,
            decimal? minPrice, decimal? maxPrice, int? minDuration, int? maxDuration, double? minRating, string? category)
        {
            var query = _context.Courses.AsQueryable();

            // Tìm kiếm theo tên khóa học hoặc chủ đề
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.Type.Contains(keyword) || c.Category.Contains(keyword));
            }

            // Tìm kiếm theo tên coach
            if (!string.IsNullOrEmpty(decription))
            {
                query = query.Where(c => c.Description.Contains(decription));
            }

            // Lọc theo mức giá
            if (minPrice.HasValue)
            {
                query = query.Where(c => c.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= maxPrice.Value);
            }

            // Lọc theo thời lượng khóa học
            if (minDuration.HasValue)
            {
                query = query.Where(c => c.Duration >= minDuration.Value);
            }
            if (maxDuration.HasValue)
            {
                query = query.Where(c => c.Duration <= maxDuration.Value);
            }

           

            // Lọc theo thể loại khóa học
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(c => c.Category.Equals(category));
            }

            return await query.ToListAsync();
        }
    }
}

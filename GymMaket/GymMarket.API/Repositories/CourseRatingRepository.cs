using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class CourseRatingRepository : GenericRepository<CourseRating, string>, ICourseRatingRepository
    {
        private readonly GymMarketContext _context;
        public CourseRatingRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

         async Task<IEnumerable<CourseRating>> ICourseRatingRepository.GetByCourseIdAsync(string courseId)
        {
            var courseRating = await (from c in _context.Courses
                                       join cor in _context.CourseRatings on c.CourseId equals cor.CourseId
                                       where c.CourseId == courseId
                                       select cor).ToListAsync();

            return courseRating;
        }
    }
}

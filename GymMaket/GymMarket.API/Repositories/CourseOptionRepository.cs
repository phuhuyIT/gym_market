using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class CourseOptionRepository : GenericRepository<CourseOption, string>, ICourseOptionRepository
    {
        private readonly GymMarketContext _context;
        public CourseOptionRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }


        public async Task<IEnumerable<CourseOption>> GetByCourseIdAsync(string courseId)
        {
            var courseOptions = await _context.CourseOptions
                .AsNoTracking()
                .Where(op => op.CourseId == courseId)
                .OrderBy(op => op.OptionName)
                .ToListAsync();

            return courseOptions;
        }

        public async Task<IEnumerable<CourseOption>> GetByTrainerIdAsync(string trainerId)
        {
            return await _context.CourseOptions
                .AsNoTracking()
                .Where(op => op.Course != null && op.Course.TrainerId == trainerId)
                .OrderBy(op => op.Course!.Title)
                .ThenBy(op => op.OptionName)
                .ToListAsync();
        }
    }
}

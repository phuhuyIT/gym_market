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
            var courseOptions = await (from c in _context.Courses
                                       join cor in _context.CourseRegistrations on c.CourseId equals cor.CourseId
                                       join cop in _context.CourseRegistrationOptions on cor.RegistrationId equals cop.RegistrationId
                                       join op in _context.CourseOptions on cop.OptionId equals op.OptionId
                                       where c.CourseId == courseId
                                       select op).ToListAsync();

            return courseOptions;
        }
    }
}

using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class LectureRepository : GenericRepository<Lecture, string>, ILectureRepository
    {
        private readonly GymMarketContext _context;
        public LectureRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public async Task<IEnumerable<Lecture>> GetLecturesByCourseIdAsync(string courseId)
        {
            return await _context.Lectures
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Order)
                .ToListAsync();
        }
    }
}

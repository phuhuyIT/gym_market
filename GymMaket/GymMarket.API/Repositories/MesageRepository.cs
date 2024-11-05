using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class MesageRepository : GenericRepository<Message, string>
    {
        private readonly GymMarketContext _context;
        public MesageRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
        // Implementation of GetTrainerMessagesByCourseIdAsync
        public async Task<IEnumerable<Message>> GetTrainerMessagesByCourseIdAsync(string trainerId, string courseId)
        {
            return await _context.Messages
                .Where(m => m.TrainerId == trainerId && m.CourseId == courseId)
                .ToListAsync();
        }

        // Implementation of GetStudentMessagesByCourseIdAsync
        public async Task<IEnumerable<Message>> GetStudentMessagesByCourseIdAsync(string studentId, string courseId)
        {
            return await _context.Messages
                .Where(m => m.StudentId == studentId && m.CourseId == courseId)
                .ToListAsync();
        }
    }
}

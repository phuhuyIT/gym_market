using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class LectureMaterialRepository : GenericRepository<LectureMaterial, string>, ILectureMaterialRepository
    {
        private readonly GymMarketContext _context;
        public LectureMaterialRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public async Task<IEnumerable<LectureMaterial>> GetMaterialsByLectureIdAsync(string lectureId)
        {
            return await _context.LectureMaterials
                .Where(lm => lm.LectureId == lectureId)
                .ToListAsync();
        }
    }
}

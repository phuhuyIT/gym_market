using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class TrainerRepository : GenericRepository<Trainer>, ITrainerRepository
    {
        private readonly GymMarketContext _context;
        public TrainerRepository(GymMarketContext context):base(context)
        {
            _context = context;
        }

        public async Task<List<Trainer>> GetTopRatedTrainersAsync(int topCount)
        {
            var trainerTop = await _context.Trainers.OrderByDescending(t => t.Rating).Take(topCount).ToListAsync();
            return trainerTop;
        }

        public async Task<List<Trainer>> GetTrainersAsync()
        {
            var trainerList = await _context.Trainers.ToListAsync();
            return trainerList;
        }
    }
}

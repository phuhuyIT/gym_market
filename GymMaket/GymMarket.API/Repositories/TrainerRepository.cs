using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Models;

namespace GymMarket.API.Repositories
{
    public class TrainerRepository : GenericRepository<Trainer, string>
    {
        public TrainerRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        { 
        }
    }
}

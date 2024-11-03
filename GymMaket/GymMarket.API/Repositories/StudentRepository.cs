using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Models;

namespace GymMarket.API.Repositories
{
    public class StudentRepository : GenericRepository<Student, string>
    {
        public StudentRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}

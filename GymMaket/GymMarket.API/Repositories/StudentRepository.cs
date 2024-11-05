using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class StudentRepository : GenericRepository<Student, string>
    {
        private readonly GymMarketContext _context;
        public StudentRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }


    }
}

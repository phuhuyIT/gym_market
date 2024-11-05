using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class PaymentRepository : GenericRepository<Payment, string>
    {
        private readonly GymMarketContext _context;
        public PaymentRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
        public async Task<IEnumerable<Payment>> GetPaymentsByStudentIdAsync(string studentId)
        {
            var payments = await _context.Payments
                .Where(p => p.StudentId == studentId)
                .ToListAsync();
            return payments;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByCourseIdAsync(string courseId)
        {
            var payments = await _context.Payments
                .Where(p => p.CourseId == courseId)
                .ToListAsync();
            return payments;
        }
    }
}

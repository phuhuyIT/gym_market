using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Payment;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class PaymentRepository : GenericRepository<Payment, string>, IPaymentRepository
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


        public async Task<List<Payment>> GetPaymentsOfCourse(string courseId)
        {
            var list = await _context.Payments.Where(p => p.CourseId == courseId).ToListAsync();
            return list;
        }

        public async Task<Payment> OkPayment(string paymentId)
        {
            var payment = await _context.Payments.Where(p => p.PaymentId == paymentId).FirstOrDefaultAsync();
            payment!.PaymentStatus = "Paid";
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> CancelPayment(CancelPayment model)
        {
            var payment = await _context.Payments.Where(p => p.PaymentId == model.PaymentId).FirstOrDefaultAsync();
            payment!.PaymentStatus = "Canceled";
            payment.Note = model.Note;

            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}

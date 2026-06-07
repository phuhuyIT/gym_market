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

        public async Task<List<GetPaymentDto>> GetPaymentsOfCourse(string courseId)
        {
            var list = await _context.Payments
                .Where(p => p.CourseId == courseId)
                .Select(p => new GetPaymentDto
                {
                    PaymentId = p.PaymentId,
                    CourseId = p.CourseId,
                    StudentId = p.StudentId,
                    PaymentAmount = p.PaymentAmount,
                    PaymentDate = p.PaymentDate,
                    PaymentStatus = p.PaymentStatus,
                    PaymentType = p.PaymentType,
                    Note = p.Note,
                    CreatedAt = p.CreatedAt,
                    FullName = p.Student!.Name
                })
                .ToListAsync();

            // Collapse any legacy "COMPLETED" rows to the canonical "Paid" the client understands.
            foreach (var dto in list)
            {
                dto.PaymentStatus = PaymentStatus.Normalize(dto.PaymentStatus);
            }

            return list;
        }

        public async Task<Payment?> OkPayment(string paymentId)
        {
            var payment = await _context.Payments.Where(p => p.PaymentId == paymentId).FirstOrDefaultAsync();
            if (payment != null)
            {
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.PaymentDate ??= DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;
                await SyncRegistrationStatusAsync(payment.StudentId, payment.CourseId, PaymentStatus.Paid);
                await _context.SaveChangesAsync();
            }
            return payment;
        }

        public async Task<Payment?> CancelPayment(CancelPayment model)
        {
            var payment = await _context.Payments.Where(p => p.PaymentId == model.PaymentId).FirstOrDefaultAsync();
            if (payment != null)
            {
                payment.PaymentStatus = PaymentStatus.Canceled;
                payment.Note = model.Note;
                payment.UpdatedAt = DateTime.UtcNow;
                await SyncRegistrationStatusAsync(payment.StudentId, payment.CourseId, PaymentStatus.Canceled);
                await _context.SaveChangesAsync();
            }
            return payment;
        }

        public async Task<Payment?> ConfirmCoursePayment(string studentId, string courseId, string paymentType)
        {
            // Reuse the pending payment created at registration instead of inserting a
            // duplicate row for the same (student, course).
            var payment = await _context.Payments
                .Where(p => p.StudentId == studentId && p.CourseId == courseId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            // Idempotent: a repeated gateway callback (or the browser-return fallback firing
            // after the IPN) must not double-record or throw.
            if (payment != null && PaymentStatus.IsPaid(payment.PaymentStatus))
                return payment;

            if (payment == null)
            {
                // No registration pre-created a payment (e.g. a direct gateway flow) — create one.
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
                payment = new Payment
                {
                    PaymentId = Guid.NewGuid().ToString(),
                    StudentId = studentId,
                    CourseId = courseId,
                    PaymentAmount = (course?.Price ?? 0) + (course?.AdditionalPrice ?? 0),
                    CreatedAt = DateTime.UtcNow,
                };
                _context.Payments.Add(payment);
            }

            payment.PaymentStatus = PaymentStatus.Paid;
            payment.PaymentType = paymentType;
            payment.PaymentDate = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            await SyncRegistrationStatusAsync(studentId, courseId, PaymentStatus.Paid);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> HasPaidForCourse(string studentId, string courseId)
        {
            return await _context.Payments.AnyAsync(p =>
                p.StudentId == studentId &&
                p.CourseId == courseId &&
                (p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.Completed));
        }

        // Keeps the CourseRegistration's status aligned with its payment. Does not save —
        // the caller's SaveChanges commits both the payment and the registration together.
        private async Task SyncRegistrationStatusAsync(string? studentId, string? courseId, string status)
        {
            if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(courseId))
                return;

            var registration = await _context.CourseRegistrations
                .FirstOrDefaultAsync(r => r.StudentId == studentId && r.CourseId == courseId);

            if (registration != null)
            {
                registration.PaymentStatus = status;
                registration.Status = status;
                registration.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}

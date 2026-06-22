using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Payment;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class PaymentRepository : GenericRepository<Payment, string>, IPaymentRepository
    {
        private readonly GymMarketContext _context;
        private readonly INotificationRepository _notificationRepository;
        public PaymentRepository(GymMarketContext context, IMapper mapper, INotificationRepository notificationRepository) : base(context, mapper)
        {
            _context = context;
            _notificationRepository = notificationRepository;
        }

        // Resolves the AppUser id of a payment's student so a notification can target them.
        private async Task<string?> GetUserIdOfStudent(string? studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return null;
            }
            return await _context.Students
                .Where(s => s.StudentId == studentId)
                .Select(s => s.UserId)
                .FirstOrDefaultAsync();
        }

        private async Task<string?> GetCourseTitle(string? courseId)
        {
            if (string.IsNullOrEmpty(courseId))
            {
                return null;
            }
            return await _context.Courses
                .Where(c => c.CourseId == courseId)
                .Select(c => c.Title)
                .FirstOrDefaultAsync();
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
                    CourseTitle = p.Course!.Title,
                    FullName = p.Student!.Name,
                    UserId = p.Student!.UserId
                })
                .ToListAsync();

            // Collapse any legacy "COMPLETED" rows to the canonical "Paid" the client understands.
            foreach (var dto in list)
            {
                dto.PaymentStatus = PaymentStatus.Normalize(dto.PaymentStatus);
            }

            return list;
        }

        public async Task<PagedResult<GetPaymentDto>> SearchPayments(
            int pageIndex = 1,
            int pageSize = Defaults.PageSize,
            string? search = null,
            string? courseId = null,
            string? studentId = null,
            string? status = null,
            string? paymentType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? trainerId = null,
            bool includeAllCourses = false)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = Defaults.PageSize;
            if (pageSize > 50) pageSize = 50;

            search = search?.Trim();
            courseId = courseId?.Trim();
            studentId = studentId?.Trim();
            status = PaymentStatus.Normalize(status?.Trim());
            paymentType = paymentType?.Trim();
            trainerId = trainerId?.Trim();

            var query = _context.Payments
                .AsNoTracking()
                .AsQueryable();

            if (!includeAllCourses)
            {
                if (string.IsNullOrWhiteSpace(trainerId))
                {
                    return new PagedResult<GetPaymentDto>
                    {
                        Items = [],
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        TotalCount = 0
                    };
                }

                query = query.Where(p => p.Course != null && p.Course.TrainerId == trainerId);
            }

            if (!string.IsNullOrWhiteSpace(courseId))
            {
                query = query.Where(p => p.CourseId == courseId);
            }

            if (!string.IsNullOrWhiteSpace(studentId))
            {
                query = query.Where(p => p.StudentId == studentId);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = status == PaymentStatus.Paid
                    ? query.Where(p => p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.Completed)
                    : query.Where(p => p.PaymentStatus == status);
            }

            if (!string.IsNullOrWhiteSpace(paymentType))
            {
                query = query.Where(p => p.PaymentType == paymentType);
            }

            if (fromDate.HasValue)
            {
                var start = fromDate.Value.Date;
                query = query.Where(p => (p.PaymentDate ?? p.CreatedAt) >= start);
            }

            if (toDate.HasValue)
            {
                var endExclusive = toDate.Value.Date.AddDays(1);
                query = query.Where(p => (p.PaymentDate ?? p.CreatedAt) < endExclusive);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    (p.PaymentStatus != null && p.PaymentStatus.Contains(search)) ||
                    (p.PaymentType != null && p.PaymentType.Contains(search)) ||
                    p.Note.Contains(search) ||
                    (p.Course != null && p.Course.Title != null && p.Course.Title.Contains(search)) ||
                    (p.Student != null && p.Student.Name != null && p.Student.Name.Contains(search)) ||
                    (p.Student != null && p.Student.Email != null && p.Student.Email.Contains(search)) ||
                    (p.Student != null && p.Student.AppUser != null && p.Student.AppUser.FullName != null && p.Student.AppUser.FullName.Contains(search)) ||
                    (p.Student != null && p.Student.AppUser != null && p.Student.AppUser.Email != null && p.Student.AppUser.Email.Contains(search)) ||
                    (p.Student != null && p.Student.AppUser != null && p.Student.AppUser.PhoneNumber != null && p.Student.AppUser.PhoneNumber.Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .ThenBy(p => p.PaymentId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new GetPaymentDto
                {
                    PaymentId = p.PaymentId,
                    CourseId = p.CourseId,
                    StudentId = p.StudentId,
                    PaymentAmount = p.PaymentAmount,
                    PaymentDate = p.PaymentDate,
                    PaymentStatus = p.PaymentStatus == PaymentStatus.Completed ? PaymentStatus.Paid : p.PaymentStatus,
                    PaymentType = p.PaymentType,
                    Note = p.Note,
                    CreatedAt = p.CreatedAt,
                    CourseTitle = p.Course != null ? p.Course.Title : null,
                    FullName = p.Student != null ? p.Student.Name : null,
                    UserId = p.Student != null ? p.Student.UserId : null
                })
                .ToListAsync();

            return new PagedResult<GetPaymentDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
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

                var userId = await GetUserIdOfStudent(payment.StudentId);
                if (userId != null)
                {
                    var courseTitle = await GetCourseTitle(payment.CourseId);
                    await _notificationRepository.NotifyUser(
                        userId,
                        NotificationTypes.Payment,
                        "Payment confirmed",
                        $"Your payment for \"{courseTitle ?? "a course"}\" has been confirmed. You now have full access.",
                        "/client/course-registration");
                }
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

                var userId = await GetUserIdOfStudent(payment.StudentId);
                if (userId != null)
                {
                    var courseTitle = await GetCourseTitle(payment.CourseId);
                    var reason = string.IsNullOrWhiteSpace(model.Note) ? "" : $" Reason: {model.Note}";
                    await _notificationRepository.NotifyUser(
                        userId,
                        NotificationTypes.Payment,
                        "Payment canceled",
                        $"Your payment for \"{courseTitle ?? "a course"}\" was canceled.{reason}",
                        "/client/course-registration");
                }
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

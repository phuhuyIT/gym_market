using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Payment;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class PaymentRepository : GenericRepository<Payment, string>, IPaymentRepository
    {
        private readonly GymMarketContext _context;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPaymentEventService _paymentEventService;
        public PaymentRepository(
            GymMarketContext context,
            IMapper mapper,
            INotificationRepository notificationRepository,
            IPaymentEventService paymentEventService) : base(context, mapper)
        {
            _context = context;
            _notificationRepository = notificationRepository;
            _paymentEventService = paymentEventService;
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
                    UserId = p.Student!.UserId,
                })
                .ToListAsync();

            await EnrichPaymentActionFlagsAsync(list);

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
                    : status == PaymentStatus.Pending
                        ? query.Where(p => ReportablePendingStatuses().Contains(p.PaymentStatus!))
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
                    UserId = p.Student != null ? p.Student.UserId : null,
                })
                .ToListAsync();

            await EnrichPaymentActionFlagsAsync(items);

            return new PagedResult<GetPaymentDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PaymentMetricsDto> GetPaymentMetricsAsync(
            string? trainerId = null,
            bool includeAllCourses = false,
            int recentCount = 5)
        {
            trainerId = trainerId?.Trim();
            if (!includeAllCourses && string.IsNullOrWhiteSpace(trainerId))
                return new PaymentMetricsDto();

            if (recentCount < 1) recentCount = 5;
            if (recentCount > 20) recentCount = 20;

            var query = _context.Payments
                .AsNoTracking()
                .Where(p => p.CourseId != null)
                .AsQueryable();

            if (!includeAllCourses)
            {
                query = query.Where(p => p.Course != null && p.Course.TrainerId == trainerId);
            }

            var paidQuery = query.Where(p => p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.Completed);
            var pendingStatuses = ReportablePendingStatuses();
            var pendingQuery = query.Where(p => pendingStatuses.Contains(p.PaymentStatus!));

            var revenueByCourse = await paidQuery
                .GroupBy(p => new
                {
                    CourseId = p.CourseId!,
                    CourseTitle = p.Course != null ? p.Course.Title : null
                })
                .Select(g => new CourseRevenueDto
                {
                    CourseId = g.Key.CourseId,
                    CourseTitle = g.Key.CourseTitle,
                    PaidRevenue = g.Sum(p => p.PaymentAmount ?? 0),
                    PaidCount = g.Count()
                })
                .OrderByDescending(c => c.PaidRevenue)
                .ThenBy(c => c.CourseTitle)
                .ToListAsync();

            var recentPaidPayments = await paidQuery
                .OrderByDescending(p => p.PaymentDate ?? p.CreatedAt)
                .ThenByDescending(p => p.CreatedAt)
                .Take(recentCount)
                .Select(p => new RecentPaidPaymentDto
                {
                    PaymentId = p.PaymentId,
                    CourseId = p.CourseId,
                    CourseTitle = p.Course != null ? p.Course.Title : null,
                    StudentId = p.StudentId,
                    StudentName = p.Student != null ? p.Student.Name : null,
                    UserId = p.Student != null ? p.Student.UserId : null,
                    PaymentAmount = p.PaymentAmount ?? 0,
                    PaymentStatus = PaymentStatus.Paid,
                    PaidAt = p.PaymentDate ?? p.CreatedAt
                })
                .ToListAsync();

            return new PaymentMetricsDto
            {
                TotalPaidRevenue = await paidQuery.SumAsync(p => p.PaymentAmount ?? 0),
                PendingAmount = await pendingQuery.SumAsync(p => p.PaymentAmount ?? 0),
                PaidCount = await paidQuery.CountAsync(),
                PendingCount = await pendingQuery.CountAsync(),
                CanceledCount = await query.CountAsync(p => p.PaymentStatus == PaymentStatus.Canceled),
                ExpiredCount = await query.CountAsync(p => p.PaymentStatus == PaymentStatus.Expired),
                UniquePaidStudentCount = await paidQuery
                    .Where(p => p.StudentId != null)
                    .Select(p => p.StudentId!)
                    .Distinct()
                    .CountAsync(),
                RevenueByCourse = revenueByCourse,
                RecentPaidPayments = recentPaidPayments
            };
        }

        public async Task<PaymentActionResultDto> OkPayment(string paymentId)
        {
            var payment = await _context.Payments.Where(p => p.PaymentId == paymentId).FirstOrDefaultAsync();
            if (payment == null)
            {
                return PaymentActionResultDto.Failure(
                    PaymentErrorCode.PaymentNotFound,
                    "Payment was not found.");
            }

            var validation = await ValidateManualApprovalAsync(payment);
            if (validation != null)
            {
                return validation;
            }

            var oldStatus = payment.PaymentStatus;
            payment.PaymentStatus = PaymentStatus.Paid;
            payment.PaymentDate ??= DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;
            await CancelOtherPendingPaymentsAsync(payment);
            await SyncRegistrationStatusAsync(payment.StudentId, payment.CourseId, PaymentStatus.Paid);
            await _paymentEventService.AddPaymentEventAsync(
                payment.PaymentId,
                PaymentEventType.ManualApproved,
                oldStatus,
                payment.PaymentStatus,
                PaymentEventSource.Trainer,
                "Trainer manually approved the payment.");
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

            return PaymentActionResultDto.Success(payment);
        }

        public async Task<PaymentActionResultDto> CancelPayment(CancelPayment model)
        {
            var payment = await _context.Payments.Where(p => p.PaymentId == model.PaymentId).FirstOrDefaultAsync();
            if (payment == null)
            {
                return PaymentActionResultDto.Failure(
                    PaymentErrorCode.PaymentNotFound,
                    "Payment was not found.");
            }

            if (!IsCancelable(payment.PaymentStatus))
            {
                return PaymentActionResultDto.Failure(
                    PaymentErrorCode.PaymentAlreadyFinalized,
                    "Only pending payments can be canceled.",
                    payment);
            }

            var oldStatus = payment.PaymentStatus;
            payment.PaymentStatus = PaymentStatus.Canceled;
            payment.Note = model.Note;
            payment.UpdatedAt = DateTime.UtcNow;

            var hasPaidPayment = await HasOtherPaidPaymentAsync(payment);
            if (!hasPaidPayment)
            {
                await SyncRegistrationStatusAsync(payment.StudentId, payment.CourseId, PaymentStatus.Canceled);
            }

            await _paymentEventService.AddPaymentEventAsync(
                payment.PaymentId,
                PaymentEventType.ManualCanceled,
                oldStatus,
                payment.PaymentStatus,
                PaymentEventSource.Trainer,
                string.IsNullOrWhiteSpace(model.Note) ? "Trainer manually canceled the payment." : model.Note);
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

            return PaymentActionResultDto.Success(payment);
        }

        public async Task<List<PaymentEventDto>> GetPaymentEventsAsync(string paymentId)
        {
            return await _context.PaymentEvents
                .AsNoTracking()
                .Where(e => e.PaymentId == paymentId)
                .OrderBy(e => e.CreatedAt)
                .ThenBy(e => e.PaymentEventId)
                .Select(e => new PaymentEventDto
                {
                    PaymentEventId = e.PaymentEventId,
                    PaymentId = e.PaymentId,
                    EventType = e.EventType,
                    OldStatus = e.OldStatus,
                    NewStatus = e.NewStatus,
                    Source = e.Source,
                    Message = e.Message,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();
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
                await _paymentEventService.AddPaymentEventAsync(
                    payment.PaymentId,
                    PaymentEventType.Created,
                    null,
                    payment.PaymentStatus,
                    PaymentEventSource.System,
                    "Direct course payment record created before confirmation.");
            }

            var oldStatus = payment.PaymentStatus;
            payment.PaymentStatus = PaymentStatus.Paid;
            payment.PaymentType = paymentType;
            payment.PaymentDate = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            await SyncRegistrationStatusAsync(studentId, courseId, PaymentStatus.Paid);
            await _paymentEventService.AddPaymentEventAsync(
                payment.PaymentId,
                PaymentEventType.Paid,
                oldStatus,
                payment.PaymentStatus,
                paymentType == PaymentType.Momo ? PaymentEventSource.Momo : PaymentEventSource.System,
                "Course payment was confirmed.");
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> ConfirmGatewayPayment(string paymentId, string paymentType)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if (payment == null)
                return null;

            if (!PaymentStatus.IsPaid(payment.PaymentStatus))
            {
                var oldStatus = payment.PaymentStatus;
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.PaymentType = paymentType;
                payment.PaymentDate = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                await CancelOtherPendingPaymentsAsync(payment);
                await SyncRegistrationStatusAsync(payment.StudentId, payment.CourseId, PaymentStatus.Paid);
                await _paymentEventService.AddPaymentEventAsync(
                    payment.PaymentId,
                    PaymentEventType.Paid,
                    oldStatus,
                    payment.PaymentStatus,
                    paymentType == PaymentType.Momo ? PaymentEventSource.Momo : PaymentEventSource.System,
                    "Gateway confirmed the payment.");
                await _context.SaveChangesAsync();
            }

            return payment;
        }

        public async Task<Payment?> CancelGatewayPayment(string paymentId, string? note)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if (payment == null)
                return null;

            if (PaymentStatus.IsPaid(payment.PaymentStatus))
                return payment;

            var oldStatus = payment.PaymentStatus;
            payment.PaymentStatus = PaymentStatus.Canceled;
            payment.Note = string.IsNullOrWhiteSpace(note) ? payment.Note : note;
            payment.UpdatedAt = DateTime.UtcNow;
            await CancelOtherPendingPaymentsAsync(payment);

            var hasPaidPayment = await _context.Payments.AnyAsync(p =>
                p.PaymentId != paymentId
                && p.StudentId == payment.StudentId
                && p.CourseId == payment.CourseId
                && (p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.Completed));

            if (!hasPaidPayment)
            {
                await SyncRegistrationStatusAsync(payment.StudentId, payment.CourseId, PaymentStatus.Canceled);
            }

            await _paymentEventService.AddPaymentEventAsync(
                payment.PaymentId,
                PaymentEventType.Canceled,
                oldStatus,
                payment.PaymentStatus,
                PaymentEventSource.Momo,
                string.IsNullOrWhiteSpace(note) ? "Gateway canceled or failed the payment." : note);
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

        private async Task CancelOtherPendingPaymentsAsync(Payment paidPayment)
        {
            var pendingPayments = await _context.Payments
                .Where(p => p.PaymentId != paidPayment.PaymentId
                    && p.StudentId == paidPayment.StudentId
                    && p.CourseId == paidPayment.CourseId
                    && ReportablePendingStatuses().Contains(p.PaymentStatus!))
                .ToListAsync();

            foreach (var pending in pendingPayments)
            {
                var oldStatus = pending.PaymentStatus;
                pending.PaymentStatus = PaymentStatus.Canceled;
                pending.Note = string.IsNullOrWhiteSpace(pending.Note)
                    ? "Canceled because another payment method completed."
                    : pending.Note;
                pending.UpdatedAt = DateTime.UtcNow;

                await _paymentEventService.AddPaymentEventAsync(
                    pending.PaymentId,
                    PaymentEventType.ReplacedBySuccessfulPayment,
                    oldStatus,
                    pending.PaymentStatus,
                    PaymentEventSource.System,
                    "Canceled because another payment attempt completed.");
            }
        }

        private static bool IsPending(string? status) =>
            status == PaymentStatus.Pending
            || status == PaymentStatus.NotStarted
            || status == PaymentStatus.PendingPayment
            || status == PaymentStatus.AwaitingConfirmation;

        private static string[] ReportablePendingStatuses() =>
        [
            PaymentStatus.Pending,
            PaymentStatus.NotStarted,
            PaymentStatus.PendingPayment,
            PaymentStatus.AwaitingConfirmation
        ];

        private static bool IsManualApprovalCandidate(string? status, string? paymentType) =>
            IsPending(status) && paymentType != PaymentType.Momo;

        private static bool IsCancelable(string? status) => IsPending(status);

        private static string? GetActionBlockedReason(string? status, string? paymentType)
        {
            if (PaymentStatus.IsPaid(status))
                return "Payment is already paid.";
            if (status == PaymentStatus.Canceled || status == PaymentStatus.Expired)
                return "Payment is no longer active.";
            if (paymentType == PaymentType.Momo)
                return "Momo payments are confirmed by the gateway callback.";
            return null;
        }

        private async Task<PaymentActionResultDto?> ValidateManualApprovalAsync(Payment payment)
        {
            if (!IsPending(payment.PaymentStatus))
            {
                return PaymentActionResultDto.Failure(
                    PaymentErrorCode.PaymentAlreadyFinalized,
                    "Only pending payments can be approved.",
                    payment);
            }

            if (payment.PaymentType == PaymentType.Momo)
            {
                return PaymentActionResultDto.Failure(
                    PaymentErrorCode.GatewayPaymentManualApprovalNotAllowed,
                    "Momo payments must be confirmed by Momo callback.",
                    payment);
            }

            if (await HasOtherPaidPaymentAsync(payment))
            {
                return PaymentActionResultDto.Failure(
                    PaymentErrorCode.PaymentObsolete,
                    "This payment attempt was replaced by another successful payment.",
                    payment);
            }

            return null;
        }

        private Task<bool> HasOtherPaidPaymentAsync(Payment payment)
        {
            return _context.Payments.AnyAsync(p =>
                p.PaymentId != payment.PaymentId
                && p.StudentId == payment.StudentId
                && p.CourseId == payment.CourseId
                && (p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.Completed));
        }

        private async Task EnrichPaymentActionFlagsAsync(IEnumerable<GetPaymentDto> payments)
        {
            foreach (var dto in payments)
            {
                dto.PaymentStatus = PaymentStatus.Normalize(dto.PaymentStatus);
                var isObsolete = false;

                if (IsPending(dto.PaymentStatus) && !string.IsNullOrEmpty(dto.StudentId) && !string.IsNullOrEmpty(dto.CourseId))
                {
                    isObsolete = await _context.Payments.AnyAsync(p =>
                        p.PaymentId != dto.PaymentId
                        && p.StudentId == dto.StudentId
                        && p.CourseId == dto.CourseId
                        && (p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.Completed));
                }

                dto.CanApprove = IsManualApprovalCandidate(dto.PaymentStatus, dto.PaymentType) && !isObsolete;
                dto.CanCancel = IsCancelable(dto.PaymentStatus) && !isObsolete;
                dto.ActionBlockedReason = isObsolete
                    ? "This payment attempt was replaced by another successful payment."
                    : GetActionBlockedReason(dto.PaymentStatus, dto.PaymentType);
            }
        }
    }
}

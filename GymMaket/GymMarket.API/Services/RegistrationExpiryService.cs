using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Services
{
    public class RegistrationExpiryService : IRegistrationExpiryService
    {
        private readonly GymMarketContext _context;
        private readonly IPaymentEventService _paymentEventService;
        private readonly INotificationRepository _notificationRepository;

        public RegistrationExpiryService(
            GymMarketContext context,
            IPaymentEventService paymentEventService,
            INotificationRepository notificationRepository)
        {
            _context = context;
            _paymentEventService = paymentEventService;
            _notificationRepository = notificationRepository;
        }

        public async Task<int> ExpireStalePendingRegistrationsAsync(
            string? courseId = null,
            string? studentId = null,
            CancellationToken cancellationToken = default)
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-Defaults.PaymentTimeoutMinutes);
            var openStatuses = PaymentStatus.OpenStatuses();

            var query = _context.CourseRegistrations
                .Where(cr => openStatuses.Contains(cr.PaymentStatus!)
                    && ((cr.UpdatedAt ?? cr.CreatedAt) ?? DateTime.MinValue) <= cutoff);

            if (!string.IsNullOrWhiteSpace(courseId))
            {
                query = query.Where(cr => cr.CourseId == courseId);
            }

            if (!string.IsNullOrWhiteSpace(studentId))
            {
                query = query.Where(cr => cr.StudentId == studentId);
            }

            var registrations = await query.ToListAsync(cancellationToken);
            if (registrations.Count == 0)
                return 0;

            var now = DateTime.UtcNow;
            foreach (var registration in registrations)
            {
                registration.Status = PaymentStatus.Expired;
                registration.PaymentStatus = PaymentStatus.Expired;
                registration.UpdatedAt = now;
            }

            var pairs = registrations
                .Where(r => !string.IsNullOrWhiteSpace(r.StudentId) && !string.IsNullOrWhiteSpace(r.CourseId))
                .Select(r => new { StudentId = r.StudentId!, CourseId = r.CourseId! })
                .Distinct()
                .ToList();

            foreach (var pair in pairs)
            {
                var payments = await _context.Payments
                    .Where(p => p.StudentId == pair.StudentId
                        && p.CourseId == pair.CourseId
                        && openStatuses.Contains(p.PaymentStatus!))
                    .ToListAsync(cancellationToken);

                foreach (var payment in payments)
                {
                    var oldStatus = payment.PaymentStatus;
                    payment.PaymentStatus = PaymentStatus.Expired;
                    payment.Note = string.IsNullOrWhiteSpace(payment.Note)
                        ? "Expired because payment was not completed in time."
                        : payment.Note;
                    payment.UpdatedAt = now;

                    await _paymentEventService.AddPaymentEventAsync(
                        payment.PaymentId,
                        PaymentEventType.Expired,
                        oldStatus,
                        payment.PaymentStatus,
                        PaymentEventSource.System,
                        "Background expiry released an unpaid course seat.",
                        cancellationToken: cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            foreach (var registration in registrations)
            {
                await NotifyStudentPaymentExpiredAsync(registration, cancellationToken);
            }

            return registrations.Count;
        }

        private async Task NotifyStudentPaymentExpiredAsync(CourseRegistration registration, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(registration.StudentId) || string.IsNullOrWhiteSpace(registration.CourseId))
                return;

            var details = await _context.CourseRegistrations
                .AsNoTracking()
                .Where(r => r.RegistrationId == registration.RegistrationId)
                .Select(r => new
                {
                    StudentUserId = r.Student != null ? r.Student.UserId : null,
                    CourseTitle = r.Course != null ? r.Course.Title : null
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(details?.StudentUserId))
                return;

            await _notificationRepository.NotifyUser(
                details.StudentUserId,
                NotificationTypes.Payment,
                "Payment expired",
                $"Your payment window for \"{details.CourseTitle ?? "a course"}\" expired. Restart payment to reserve a seat again.",
                $"/client/course-payment/{registration.CourseId}");
        }
    }
}

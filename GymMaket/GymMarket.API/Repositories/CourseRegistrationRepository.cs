using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.DTOs.FileMinIO;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using GymMarket.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace GymMarket.API.Repositories
{
    public class CourseRegistrationRepository : GenericRepository<CourseRegistration, string>, ICourseRegistrationRepository
    {
        private readonly GymMarketContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;
        private readonly IRegistrationExpiryService _registrationExpiryService;
        private readonly IPaymentEventService _paymentEventService;

        public CourseRegistrationRepository(
            GymMarketContext context,
            IMapper mapper,
            INotificationRepository notificationRepository,
            IRegistrationExpiryService registrationExpiryService,
            IPaymentEventService paymentEventService) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _registrationExpiryService = registrationExpiryService;
            _paymentEventService = paymentEventService;
        }

        // Registers a course for a student and initializes status as 'Pending Payment'
        public async Task<RegisterCourseResultDto> RegisterCourseAsync(RegisterCourseDto dto, string studentId)
        {
            await _registrationExpiryService.ExpireStalePendingRegistrationsAsync(dto.CourseId);

            if (_context.Database.IsRelational())
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                await AcquireCourseRegistrationLockAsync(dto.CourseId);

                var result = await RegisterCourseCoreAsync(dto, studentId);
                if (result.Success)
                {
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                }

                return result;
            }

            return await RegisterCourseCoreAsync(dto, studentId);
        }

        private async Task<RegisterCourseResultDto> RegisterCourseCoreAsync(RegisterCourseDto dto, string studentId)
        {
            _context.ChangeTracker.Clear();

            var courseExists = await _context.CourseRegistrations
                .Where(cr => cr.StudentId == studentId && cr.CourseId == dto.CourseId)
                .OrderByDescending(cr => cr.UpdatedAt ?? cr.CreatedAt)
                .FirstOrDefaultAsync();

            if(courseExists != null && !IsRetryable(courseExists))
            {
                return RegisterCourseResultDto.Ok(courseExists);
            }

            var course = await _context.Courses.Where(c => c.CourseId == dto.CourseId).FirstOrDefaultAsync();
            if (course == null)
            {
                return RegisterCourseResultDto.Fail(CourseRegistrationErrorCode.CourseNotFound);
            }

            if (!string.IsNullOrWhiteSpace(course.Status) && course.Status != CourseStatus.Published)
            {
                return RegisterCourseResultDto.Fail(CourseRegistrationErrorCode.CourseNotPublished);
            }

            if (await IsCourseFullAsync(dto.CourseId))
            {
                return RegisterCourseResultDto.Fail(CourseRegistrationErrorCode.CourseFull);
            }

            var selectedOptions = await GetSelectedCourseOptionsAsync(dto.CourseId, dto.OptionIds);
            if (selectedOptions == null)
            {
                return RegisterCourseResultDto.Fail(CourseRegistrationErrorCode.InvalidCourseOption);
            }

            var isRetry = courseExists != null;
            var now = DateTime.UtcNow;
            var registration = courseExists ?? new CourseRegistration
            {
                RegistrationId = Guid.NewGuid().ToString(),
                CourseId = dto.CourseId,
                StudentId = studentId,
            };

            registration.Status = PaymentStatus.PendingPayment;
            registration.PaymentStatus = PaymentStatus.NotStarted;
            registration.InitialPayment = null;
            registration.UpdatedAt = now;
            registration.CreatedAt ??= now;

            if (courseExists == null)
            {
                _context.CourseRegistrations.Add(registration);
            }
            else
            {
                var previousOptions = await _context.CourseRegistrationOptions
                    .Where(o => o.RegistrationId == registration.RegistrationId)
                    .ToListAsync();
                _context.CourseRegistrationOptions.RemoveRange(previousOptions);
            }

            foreach (var option in selectedOptions)
            {
                _context.CourseRegistrationOptions.Add(new CourseRegistrationOption
                {
                    RegistrationOptionId = Guid.NewGuid().ToString(),
                    RegistrationId = registration.RegistrationId,
                    OptionId = option.OptionId
                });
            }

            var optionAmount = selectedOptions.Sum(o => o.Price ?? 0);
            var paymentAmount = (course.Price ?? 0) + (course.AdditionalPrice ?? 0) + optionAmount;

            var paymentId = Guid.NewGuid().ToString();
            var payment = new Payment()
            {
                CourseId = dto.CourseId,
                PaymentAmount = paymentAmount,
                CreatedAt = now,
                PaymentDate = now,
                PaymentId = paymentId,
                PaymentStatus = PaymentStatus.Pending,
                PaymentType = PaymentType.BankTransfer,
                // Short transfer reference the student puts in their bank transfer memo
                // and the trainer matches against in the payments list (shown as Note).
                Note = BuildTransferReference(paymentId),
                StudentId = studentId,
                UpdatedAt = now,
            };

            _context.Payments.Add(payment);
            await _paymentEventService.AddPaymentEventAsync(
                payment.PaymentId,
                PaymentEventType.Created,
                null,
                payment.PaymentStatus,
                PaymentEventSource.Student,
                "Course registration created a bank-transfer payment attempt.");
            await _context.SaveChangesAsync();
            await NotifyTrainerBankTransferStartedAsync(course.CourseId, course.Title, studentId, isRetry);

            return RegisterCourseResultDto.Ok(registration);
        }

        private async Task AcquireCourseRegistrationLockAsync(string courseId)
        {
            if (!_context.Database.IsSqlServer())
                return;

            var transaction = _context.Database.CurrentTransaction;
            if (transaction == null)
                return;

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.Transaction = transaction.GetDbTransaction();
            command.CommandText = @"
DECLARE @result int;
EXEC @result = sp_getapplock
    @Resource = @resource,
    @LockMode = 'Exclusive',
    @LockOwner = 'Transaction',
    @LockTimeout = 10000;
IF @result < 0
BEGIN
    THROW 51000, 'Could not acquire course registration lock.', 1;
END";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@resource";
            parameter.Value = $"course-registration:{courseId}";
            command.Parameters.Add(parameter);

            await command.ExecuteNonQueryAsync();
        }

        // Initializes the payment, setting a timestamp for tracking the 5-minute window.
        // Only the student who owns the registration may touch it.
        public async Task<bool> InitializePaymentAsync(string registrationId, decimal initialPayment, string studentId)
        {
            var registration = await _context.CourseRegistrations.FindAsync(registrationId);

            if (registration == null || registration.StudentId != studentId || registration.PaymentStatus != PaymentStatus.NotStarted)
                return false;

            registration.InitialPayment = initialPayment;
            registration.PaymentStatus = PaymentStatus.Pending;
            registration.UpdatedAt = DateTime.UtcNow;

            // Update the registration in the database
            _context.CourseRegistrations.Update(registration);
            await _context.SaveChangesAsync();

            return true;
        }

        // Cancels a registration if the payment is not completed within 5 minutes.
        // Only the student who owns the registration may cancel it.
        public async Task<bool> CancelRegistrationAsync(string registrationId, string studentId)
        {
            var registration = await _context.CourseRegistrations.FindAsync(registrationId);

            if (registration == null || registration.StudentId != studentId || !PaymentStatus.IsOpen(registration.PaymentStatus))
                return false;

            // Check if more than 5 minutes have passed since payment initialization
            var currentTime = DateTime.UtcNow;
            var timeElapsed = currentTime - registration.UpdatedAt;
            if (timeElapsed > TimeSpan.FromMinutes(Defaults.PaymentTimeoutMinutes))
            {
                // Discard registration by updating status and removing from database if needed
                registration.Status = PaymentStatus.Canceled;
                registration.PaymentStatus = PaymentStatus.Expired;
                registration.UpdatedAt = currentTime;

                await ExpireOpenPaymentsAsync(registration.StudentId, registration.CourseId, currentTime);

                _context.CourseRegistrations.Update(registration);
                await _context.SaveChangesAsync();
                await NotifyStudentPaymentExpiredAsync(registration.StudentId, registration.CourseId);

                return true;
            }
            return false;
        }

        public async Task<List<GetCourseDto>> GetCourseRegistrations(string studentId)
        {
            await _registrationExpiryService.ExpireStalePendingRegistrationsAsync(studentId: studentId);

            var registrations = await _context.CourseRegistrations
                .Where(cr => cr.StudentId == studentId)
                .Include(cr => cr.Course!)
                    .ThenInclude(c => c.FileCourses)
                .ToListAsync();

            // Index this student's payments by course so each registration shows ITS OWN
            // payment status. The previous join matched payments on CourseId alone, so a
            // student could inherit another student's payment status for the same course
            // (and the join also duplicated course rows, one per payment).
            var statusByCourse = (await _context.Payments
                    .Where(p => p.StudentId == studentId && p.CourseId != null)
                    .ToListAsync())
                .GroupBy(p => p.CourseId!)
                .ToDictionary(
                    g => g.Key,
                    // Prefer a paid record, otherwise the most recent one.
                    g => g.OrderByDescending(p => PaymentStatus.IsPaid(p.PaymentStatus))
                          .ThenByDescending(p => p.CreatedAt)
                          .First().PaymentStatus);

            var courseDtos = registrations
                .Where(cr => cr.Course != null)
                .Select(cr =>
                {
                    var dto = _mapper.Map<Course, GetCourseDto>(cr.Course!);
                    statusByCourse.TryGetValue(cr.CourseId!, out var status);
                    dto.StatusPayment = PaymentStatus.Normalize(status);
                    return dto;
                })
                .ToList();

            return courseDtos;
        }

        // Payment details for ONE course the student has registered for. Returns null
        // when the student has no registration for the course (so the controller 404s)
        // — a student can never read payment info for a course they didn't register for.
        public async Task<CoursePaymentInfoDto?> GetCoursePaymentInfo(string studentId, string courseId)
        {
            await _registrationExpiryService.ExpireStalePendingRegistrationsAsync(courseId, studentId);

            var registration = await _context.CourseRegistrations
                .FirstOrDefaultAsync(cr => cr.StudentId == studentId && cr.CourseId == courseId);

            if (registration == null)
                return null;

            var course = await _context.Courses
                .Include(c => c.Trainer)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
                return null;

            // Prefer a paid payment, else the most recent, mirroring GetCourseRegistrations.
            var payment = (await _context.Payments
                    .Where(p => p.StudentId == studentId && p.CourseId == courseId)
                    .ToListAsync())
                .OrderByDescending(p => PaymentStatus.IsPaid(p.PaymentStatus))
                .ThenByDescending(p => p.CreatedAt)
                .FirstOrDefault();

            var trainer = course.Trainer;
            var bankConfigured = trainer != null
                && !string.IsNullOrWhiteSpace(trainer.BankBin)
                && !string.IsNullOrWhiteSpace(trainer.BankAccountNo);

            var selectedOptions = await GetRegistrationOptionsAsync(registration.RegistrationId);
            var courseAmount = (course.Price ?? 0) + (course.AdditionalPrice ?? 0);
            var optionsAmount = selectedOptions.Sum(o => o.Price);
            var currentPrice = courseAmount + optionsAmount;

            // The amount snapshot is locked once the student has paid (they owe exactly
            // what they paid). While the payment is still open, keep it in sync with the
            // live course price so a trainer who changes the price before the student pays
            // is reflected in the QR amount — and the trainer's payments list agrees.
            decimal amount;
            if (payment == null)
            {
                amount = currentPrice;
            }
            else if (PaymentStatus.IsPaid(payment.PaymentStatus))
            {
                amount = payment.PaymentAmount ?? currentPrice;
            }
            else if (payment.PaymentStatus == PaymentStatus.Pending
                  || payment.PaymentStatus == PaymentStatus.NotStarted)
            {
                amount = currentPrice;
                if (payment.PaymentAmount != currentPrice)
                {
                    payment.PaymentAmount = currentPrice;
                    payment.UpdatedAt = DateTime.UtcNow;
                    _context.Payments.Update(payment);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Canceled / expired: show the recorded amount, untouched.
                amount = payment.PaymentAmount ?? currentPrice;
            }

            return new CoursePaymentInfoDto
            {
                PaymentId = payment?.PaymentId,
                CourseId = courseId,
                CourseTitle = course.Title,
                Amount = amount,
                CourseAmount = courseAmount,
                OptionsAmount = optionsAmount,
                Options = selectedOptions,
                Status = PaymentStatus.Normalize(payment?.PaymentStatus),
                Reference = payment?.Note,
                BankBin = trainer?.BankBin,
                BankAccountNo = trainer?.BankAccountNo,
                BankAccountName = trainer?.BankAccountName,
                TrainerName = trainer?.Name,
                BankConfigured = bankConfigured,
            };
        }

        // The student tapped "I've paid" on the payment screen. We deliberately do NOT
        // mark the payment paid here — the trainer still verifies the bank transfer and
        // approves it (PaymentRepository.OkPayment). All this does is ping the trainer so
        // they know a transfer is waiting, and echo back the current payment info. Returns
        // null when the caller has no registration for the course (controller 404s).
        public async Task<CoursePaymentInfoDto?> ConfirmPaymentByStudent(string studentId, string courseId)
        {
            await _registrationExpiryService.ExpireStalePendingRegistrationsAsync(courseId, studentId);

            var registration = await _context.CourseRegistrations
                .FirstOrDefaultAsync(cr => cr.StudentId == studentId && cr.CourseId == courseId);

            if (registration == null)
                return null;

            var course = await _context.Courses
                .Include(c => c.Trainer)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
                return null;

            var payment = (await _context.Payments
                    .Where(p => p.StudentId == studentId && p.CourseId == courseId)
                    .ToListAsync())
                .OrderByDescending(p => PaymentStatus.IsPaid(p.PaymentStatus))
                .ThenByDescending(p => p.CreatedAt)
                .FirstOrDefault();

            // Only nudge the trainer while the payment is still active — no point asking
            // them to confirm something already paid or canceled. When a student says they
            // transferred money, move the record out of the short checkout expiry window.
            var isOpen = payment != null
                && (payment.PaymentStatus == PaymentStatus.Pending
                 || payment.PaymentStatus == PaymentStatus.NotStarted
                 || payment.PaymentStatus == PaymentStatus.PendingPayment
                 || payment.PaymentStatus == PaymentStatus.AwaitingConfirmation);

            var trainerUserId = course.Trainer?.UserId;
            if (isOpen && !string.IsNullOrEmpty(trainerUserId))
            {
                if (payment!.PaymentStatus != PaymentStatus.AwaitingConfirmation)
                {
                    var oldStatus = payment.PaymentStatus;
                    payment.PaymentStatus = PaymentStatus.AwaitingConfirmation;
                    payment.UpdatedAt = DateTime.UtcNow;
                    registration.PaymentStatus = PaymentStatus.AwaitingConfirmation;
                    registration.Status = PaymentStatus.AwaitingConfirmation;
                    registration.UpdatedAt = DateTime.UtcNow;

                    await _paymentEventService.AddPaymentEventAsync(
                        payment.PaymentId,
                        PaymentEventType.ManualSubmitted,
                        oldStatus,
                        payment.PaymentStatus,
                        PaymentEventSource.Student,
                        "Student marked the bank transfer as paid and requested trainer confirmation.");
                }

                var studentName = await _context.Students
                    .Where(s => s.StudentId == studentId)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync();

                // Upsert keyed on the per-student link: repeated taps refresh one notice
                // instead of stacking, and opening it filters the payments page to this
                // student so the trainer can verify and approve in one step.
                await _notificationRepository.NotifyUserUpsert(
                    trainerUserId,
                    NotificationTypes.Payment,
                    "Payment awaiting confirmation",
                    $"{studentName ?? "A student"} marked their payment for \"{course.Title ?? "a course"}\" as paid. Please verify the transfer and confirm.",
                    $"/agency/payments?studentId={studentId}");

                await _context.SaveChangesAsync();
            }

            return await GetCoursePaymentInfo(studentId, courseId);
        }

        // Short, bank-memo-friendly transfer reference (alphanumeric, uppercase).
        private static string BuildTransferReference(string paymentId)
        {
            var compact = paymentId.Replace("-", "");
            var suffix = compact.Length >= 8 ? compact.Substring(0, 8) : compact;
            return $"GYM{suffix.ToUpperInvariant()}";
        }

        private static bool IsRetryable(CourseRegistration registration)
        {
            var status = PaymentStatus.Normalize(registration.PaymentStatus);
            return status == PaymentStatus.Canceled || status == PaymentStatus.Expired;
        }

        private async Task<List<CourseOption>?> GetSelectedCourseOptionsAsync(string courseId, IEnumerable<string>? optionIds)
        {
            var selectedIds = (optionIds ?? [])
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.Ordinal)
                .ToList();

            if (selectedIds.Count == 0)
                return [];

            var options = await _context.CourseOptions
                .Where(o => selectedIds.Contains(o.OptionId) && o.CourseId == courseId)
                .ToListAsync();

            return options.Count == selectedIds.Count ? options : null;
        }

        private async Task<List<CoursePaymentOptionDto>> GetRegistrationOptionsAsync(string registrationId)
        {
            return await _context.CourseRegistrationOptions
                .Where(ro => ro.RegistrationId == registrationId)
                .Select(ro => new CoursePaymentOptionDto
                {
                    OptionId = ro.OptionId ?? string.Empty,
                    OptionName = ro.Option != null ? ro.Option.OptionName : null,
                    Price = ro.Option != null ? ro.Option.Price ?? 0 : 0
                })
                .ToListAsync();
        }

        private async Task<bool> IsCourseFullAsync(string courseId)
        {
            await _registrationExpiryService.ExpireStalePendingRegistrationsAsync(courseId);

            var course = await _context.Courses
                .Where(c => c.CourseId == courseId)
                .Select(c => new { c.MaxParticipants })
                .FirstOrDefaultAsync();

            if (course?.MaxParticipants == null || course.MaxParticipants <= 0)
                return false;

            var activeRegistrations = await _context.CourseRegistrations
                .Where(cr => cr.CourseId == courseId
                    && cr.PaymentStatus != PaymentStatus.Canceled
                    && cr.PaymentStatus != PaymentStatus.Expired)
                .Select(cr => cr.StudentId)
                .Distinct()
                .CountAsync();

            return activeRegistrations >= course.MaxParticipants;
        }

        private async Task ExpireOpenPaymentsAsync(string? studentId, string? courseId, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(courseId))
                return;

            var openStatuses = PaymentStatus.OpenStatuses();
            var payments = await _context.Payments
                .Where(p => p.StudentId == studentId
                    && p.CourseId == courseId
                    && openStatuses.Contains(p.PaymentStatus!))
                .ToListAsync();

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
                    "Payment expired when the registration timeout elapsed.");
            }
        }

        private async Task NotifyTrainerBankTransferStartedAsync(string courseId, string? courseTitle, string studentId, bool isRetry)
        {
            var trainerUserId = await _context.Courses
                .Where(c => c.CourseId == courseId)
                .Select(c => c.Trainer != null ? c.Trainer.UserId : null)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(trainerUserId))
                return;

            var studentName = await _context.Students
                .Where(s => s.StudentId == studentId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            var title = isRetry ? "Payment restarted" : "New payment pending";
            var action = isRetry ? "restarted payment" : "started a bank-transfer payment";
            await _notificationRepository.NotifyUserUpsert(
                trainerUserId,
                NotificationTypes.Payment,
                title,
                $"{studentName ?? "A student"} {action} for \"{courseTitle ?? "a course"}\".",
                $"/agency/payments?studentId={studentId}");
        }

        private async Task NotifyStudentPaymentExpiredAsync(string? studentId, string? courseId)
        {
            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(courseId))
                return;

            var details = await _context.CourseRegistrations
                .AsNoTracking()
                .Where(r => r.StudentId == studentId && r.CourseId == courseId)
                .Select(r => new
                {
                    StudentUserId = r.Student != null ? r.Student.UserId : null,
                    CourseTitle = r.Course != null ? r.Course.Title : null
                })
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(details?.StudentUserId))
                return;

            await _notificationRepository.NotifyUser(
                details.StudentUserId,
                NotificationTypes.Payment,
                "Payment expired",
                $"Your payment window for \"{details.CourseTitle ?? "a course"}\" expired. Restart payment to reserve a seat again.",
                $"/client/course-payment/{courseId}");
        }

    }
}

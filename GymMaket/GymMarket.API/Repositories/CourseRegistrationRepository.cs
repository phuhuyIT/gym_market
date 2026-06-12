using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.DTOs.FileMinIO;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class CourseRegistrationRepository : GenericRepository<CourseRegistration, string>, ICourseRegistrationRepository
    {
        private readonly GymMarketContext _context;
        private readonly IMapper _mapper;

        public CourseRegistrationRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Registers a course for a student and initializes status as 'Pending Payment'
        public async Task<CourseRegistration?> RegisterCourseAsync(RegisterCourseDto dto)
        {
            var courseExists = await _context.CourseRegistrations
                .Where(cr => cr.StudentId == dto.StudentId && cr.CourseId == dto.CourseId)
                .FirstOrDefaultAsync();

            if(courseExists != null)
            {
                return courseExists;
            }

            var registration = new CourseRegistration
            {
                RegistrationId = Guid.NewGuid().ToString(),
                CourseId = dto.CourseId,
                StudentId = dto.StudentId,
                Status = PaymentStatus.PendingPayment,
                PaymentStatus = PaymentStatus.NotStarted,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add the registration to the database
            _context.CourseRegistrations.Add(registration);


            var course = await _context.Courses.Where(c => c.CourseId == dto.CourseId).FirstOrDefaultAsync();

            var payment = new Payment()
            {
                CourseId = dto.CourseId,
                PaymentAmount = course!.Price + course.AdditionalPrice,
                CreatedAt = DateTime.UtcNow,
                PaymentDate = DateTime.UtcNow,
                PaymentId = Guid.NewGuid().ToString(),
                PaymentStatus = PaymentStatus.Pending,
                PaymentType = "",
                StudentId = dto.StudentId,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return registration;
        }

        // Initializes the payment, setting a timestamp for tracking the 5-minute window
        public async Task<bool> InitializePaymentAsync(string registrationId, decimal initialPayment)
        {
            var registration = await _context.CourseRegistrations.FindAsync(registrationId);

            if (registration == null || registration.PaymentStatus != PaymentStatus.NotStarted)
                return false;

            registration.InitialPayment = initialPayment;
            registration.PaymentStatus = PaymentStatus.Pending;
            registration.UpdatedAt = DateTime.UtcNow;

            // Update the registration in the database
            _context.CourseRegistrations.Update(registration);
            await _context.SaveChangesAsync();

            return true;
        }

        // Cancels a registration if the payment is not completed within 5 minutes
        public async Task<bool> CancelRegistrationAsync(string registrationId)
        {
            var registration = await _context.CourseRegistrations.FindAsync(registrationId);

            if (registration == null || registration.PaymentStatus != PaymentStatus.Pending)
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

                _context.CourseRegistrations.Update(registration);
                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<List<GetCourseDto>> GetCourseRegistrations(string studentId)
        {
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

    }
}

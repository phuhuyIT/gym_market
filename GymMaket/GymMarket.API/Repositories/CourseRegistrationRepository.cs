using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.Models;

namespace GymMarket.API.Repositories
{
    public class CourseRegistrationRepository : GenericRepository<CourseRegistration, string>
    {
        private readonly GymMarketContext _context;
        public CourseRegistrationRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public async Task<CourseRegistration?> RegisterCourseAsync(string courseId, string studentId, CourseRegistration registration)
        {
            registration.CourseId = courseId;
            registration.StudentId = studentId;
            registration.Status = "Pending Payment";
            registration.PaymentStatus = "Not Started";
            registration.CreatedAt = DateTime.UtcNow;
            registration.UpdatedAt = DateTime.UtcNow;

            await _context.CourseRegistrations.AddAsync(registration);
            await _context.SaveChangesAsync();

            return registration;
        }

        public async Task<bool> CancelRegistrationAsync(string registrationId)
        {
            var registration = await _context.CourseRegistrations.FindAsync(registrationId);

            if (registration == null || registration.PaymentStatus != "Pending")
                return false;

            var currentTime = DateTime.UtcNow;
            var timeElapsed = currentTime - registration.UpdatedAt;
            if (timeElapsed > TimeSpan.FromMinutes(5))
            {
                registration.Status = "Cancelled";
                registration.PaymentStatus = "Expired";
                registration.UpdatedAt = currentTime;

                _context.CourseRegistrations.Update(registration);
                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<bool> InitializePaymentAsync(string registrationId, decimal initialPayment)
        {
            var registration = await _context.CourseRegistrations.FindAsync(registrationId);

            if (registration == null || registration.PaymentStatus != "Not Started")
                return false;

            registration.InitialPayment = initialPayment;
            registration.PaymentStatus = "Pending";
            registration.UpdatedAt = DateTime.UtcNow;

            _context.CourseRegistrations.Update(registration);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

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
    public class CourseRegistrationRepository : GenericRepository<CourseRegistration, string>
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
            var registration = new CourseRegistration();

            // Set initial properties for the registration
            registration.CourseId = dto.CourseId;
            registration.StudentId = dto.StudentId;
            registration.Status = "Pending Payment";
            registration.PaymentStatus = "Not Started";
            registration.CreatedAt = DateTime.UtcNow;
            registration.UpdatedAt = DateTime.UtcNow;
            registration.RegistrationId = Guid.NewGuid().ToString();

            // Add the registration to the database
            await _context.CourseRegistrations.AddAsync(registration);
            await _context.SaveChangesAsync();

            return registration;
        }

        // Initializes the payment, setting a timestamp for tracking the 5-minute window
        public async Task<bool> InitializePaymentAsync(string registrationId, decimal initialPayment)
        {
            var registration = await _context.CourseRegistrations.FindAsync(registrationId);

            if (registration == null || registration.PaymentStatus != "Not Started")
                return false;

            registration.InitialPayment = initialPayment;
            registration.PaymentStatus = "Pending";
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

            if (registration == null || registration.PaymentStatus != "Pending")
                return false;

            // Check if more than 5 minutes have passed since payment initialization
            var currentTime = DateTime.UtcNow;
            var timeElapsed = currentTime - registration.UpdatedAt;
            if (timeElapsed > TimeSpan.FromMinutes(5))
            {
                // Discard registration by updating status and removing from database if needed
                registration.Status = "Cancelled";
                registration.PaymentStatus = "Expired";
                registration.UpdatedAt = currentTime;

                _context.CourseRegistrations.Update(registration);
                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<List<GetCourseDto>> GetCourseRegistrations (string studentId)
        {
            //List<CourseRegistration> list = await _context.CourseRegistrations
            //    .AsNoTrackingWithIdentityResolution()
            //    .Where(c => c.StudentId == studentId)
            //    .ToListAsync();

            var list = await (from cr in _context.CourseRegistrations
                               join c in _context.Courses on cr.CourseId equals c.CourseId
                               where cr.StudentId == studentId
                               select c).ToListAsync();
            var courseDto = _mapper.Map<List<Course>, List<GetCourseDto>>(list);

            foreach (var c in courseDto)
            {
                var courseFiles = await _context.FileCourses
                   .AsNoTrackingWithIdentityResolution()
                   .Where(course => course.CourseId == c.CourseId)
                   .ToListAsync();

                var courseFileImages = courseFiles.Where(c => c.TypeFile == "IMAGE").ToList();

                var courseFileDtos = _mapper.Map<List<FileCourse>, List<GetFileDto>>(courseFileImages);
                c.GetFileDtos = courseFileDtos;
            }
            return courseDto;
        }
    }
}

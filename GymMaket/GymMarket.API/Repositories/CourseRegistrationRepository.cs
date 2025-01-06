using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseRegistration;
using GymMarket.API.DTOs.FileMinIO;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Minio.DataModel.ILM;
using System.Dynamic;

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

            var registration = new CourseRegistration();

            // Set initial properties for the registration

            // Set default RegistrationId if it's not already set
            if (string.IsNullOrWhiteSpace(registration.RegistrationId))
            {
                registration.RegistrationId = Guid.NewGuid().ToString(); // Use a unique GUID
            }

            registration.CourseId = dto.CourseId;
            registration.StudentId = dto.StudentId;
            registration.Status = "Pending Payment";
            registration.PaymentStatus = "Not Started";
            registration.CreatedAt = DateTime.UtcNow;
            registration.UpdatedAt = DateTime.UtcNow;
            registration.RegistrationId = Guid.NewGuid().ToString();

            // Add the registration to the database
            _context.CourseRegistrations.Add(registration);


            var course = await _context.Courses.Where(c => c.CourseId == dto.CourseId).FirstOrDefaultAsync();

            var payment = new Payment()
            {
                CourseId = dto.CourseId,
                PaymentAmount = course!.Price + course.AdditionalPrice,
                CreatedAt = DateTime.Now,
                PaymentDate = DateTime.Now,
                PaymentId = Guid.NewGuid().ToString(),
                PaymentStatus = "Pending",
                PaymentType = "",
                StudentId = dto.StudentId,
                UpdatedAt = DateTime.Now,
            };

            _context.Payments.Add(payment);
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

        public async Task<List<GetCourseDto>> GetCourseRegistrations(string studentId)
        {
            //List<CourseRegistration> list = await _context.CourseRegistrations
            //    .AsNoTrackingWithIdentityResolution()
            //    .Where(c => c.StudentId == studentId)
            //    .ToListAsync();

            var courseDto = await (from cr in _context.CourseRegistrations
                                   join c in _context.Courses on cr.CourseId equals c.CourseId
                                   join p in _context.Payments on c.CourseId equals p.CourseId into paymentGroup
                                   from p in paymentGroup.DefaultIfEmpty() // Lấy nhóm Payments và cho phép null
                                   where cr.StudentId == studentId
                                   group new { c, p } by c.CourseId into grouped
                                   select new GetCourseDto
                                   {
                                       CourseId = grouped.Key,
                                       TrainerId = grouped.First().c.TrainerId,
                                       Title = grouped.First().c.Title,
                                       Description = grouped.First().c.Description,
                                       Type = grouped.First().c.Type,
                                       Category = grouped.First().c.Category,
                                       Price = grouped.First().c.Price,
                                       AdditionalPrice = grouped.First().c.AdditionalPrice,
                                       StartDate = grouped.First().c.StartDate,
                                       EndDate = grouped.First().c.EndDate,
                                       Duration = grouped.First().c.Duration,
                                       MaxParticipants = grouped.First().c.MaxParticipants,
                                       StatusPayment = grouped.FirstOrDefault().p.PaymentStatus, // Lấy Payment đầu tiên nếu có
                                       Rating = grouped.First().c.Rating
                                   }).ToListAsync();




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

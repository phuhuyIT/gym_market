using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Response.Student;
using GymMarket.API.DTOs.Student;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Repositories
{
    public class StudentRepository : GenericRepository<Student, string>, IStudentRepository
    {
        private readonly GymMarketContext _context;
        public StudentRepository(GymMarketContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public async Task<GetStudentProfileResponse> GetStudentProfileByUserId(string userId)
        {
            var student = await _context.Students
                .Include(s => s.AppUser)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return new GetStudentProfileResponse
                {
                    Errors = ["STUDENT_NOT_FOUND"],
                    StatusCode = 404,
                    Success = false
                };
            }

            var profile = new StudentProfileDto
            {
                StudentId = student.StudentId,
                UserId = student.UserId,
                ProfilePicture = student.ProfilePicture,
                HealthStatus = student.HealthStatus,
                CreatedAt = student.CreatedAt,
                UpdatedAt = student.UpdatedAt,
                FullName = student.AppUser?.FullName,
                Email = student.AppUser?.Email,
                PhoneNumber = student.AppUser?.PhoneNumber,
                Address = student.AppUser?.Address,
                Avatar = student.AppUser?.Avatar,
                Status = student.AppUser?.Status
            };

            return new GetStudentProfileResponse
            {
                Errors = [],
                StatusCode = 200,
                Success = true,
                StudentProfile = profile
            };
        }
    }
}

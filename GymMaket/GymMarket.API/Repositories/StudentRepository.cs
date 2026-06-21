using AutoMapper;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Response;
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

        public async Task<PagedResult<StudentSearchDto>> SearchStudents(
            int pageIndex = 1,
            int pageSize = Defaults.PageSize,
            string? search = null,
            string? trainerId = null,
            bool includeAllStudents = false)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = Defaults.PageSize;
            if (pageSize > 50) pageSize = 50;

            search = search?.Trim();
            trainerId = trainerId?.Trim();

            var query = _context.Students
                .AsNoTracking()
                .AsQueryable();

            if (!includeAllStudents)
            {
                if (string.IsNullOrWhiteSpace(trainerId))
                {
                    return new PagedResult<StudentSearchDto>
                    {
                        Items = [],
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        TotalCount = 0
                    };
                }

                query = query.Where(s =>
                    s.CourseRegistrations.Any(r => r.Course != null && r.Course.TrainerId == trainerId) ||
                    s.Payments.Any(p => p.Course != null && p.Course.TrainerId == trainerId));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s =>
                    (s.Name != null && s.Name.Contains(search)) ||
                    (s.Email != null && s.Email.Contains(search)) ||
                    (s.HealthStatus != null && s.HealthStatus.Contains(search)) ||
                    (s.AppUser != null && s.AppUser.FullName != null && s.AppUser.FullName.Contains(search)) ||
                    (s.AppUser != null && s.AppUser.Email != null && s.AppUser.Email.Contains(search)) ||
                    (s.AppUser != null && s.AppUser.PhoneNumber != null && s.AppUser.PhoneNumber.Contains(search)) ||
                    (s.AppUser != null && s.AppUser.Status != null && s.AppUser.Status.Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(s => s.AppUser != null ? s.AppUser.FullName : s.Name)
                .ThenBy(s => s.StudentId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudentSearchDto
                {
                    StudentId = s.StudentId,
                    UserId = s.UserId,
                    Name = s.Name,
                    Email = s.Email,
                    FullName = s.AppUser != null ? s.AppUser.FullName : null,
                    PhoneNumber = s.AppUser != null ? s.AppUser.PhoneNumber : null,
                    HealthStatus = s.HealthStatus,
                    ProfilePicture = s.ProfilePicture,
                    Status = s.AppUser != null ? s.AppUser.Status : null,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<StudentSearchDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}

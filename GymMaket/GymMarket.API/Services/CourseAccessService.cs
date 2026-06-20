using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Services
{
    public class CourseAccessService : ICourseAccessService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly GymMarketContext _context;

        public CourseAccessService(IPaymentRepository paymentRepository, GymMarketContext context)
        {
            _paymentRepository = paymentRepository;
            _context = context;
        }

        public Task<bool> CanAccessCourseAsync(ClaimsPrincipal user, string courseId)
            => AuthorizeCourseAsync(user, courseId, allowPaidStudent: true);

        public Task<bool> CanManageCourseAsync(ClaimsPrincipal user, string courseId)
            => AuthorizeCourseAsync(user, courseId, allowPaidStudent: false);

        public async Task<bool> CanAccessLectureAsync(ClaimsPrincipal user, string lectureId)
        {
            var courseId = await GetCourseIdForLectureAsync(lectureId);
            return courseId != null && await CanAccessCourseAsync(user, courseId);
        }

        public async Task<bool> CanManageLectureAsync(ClaimsPrincipal user, string lectureId)
        {
            var courseId = await GetCourseIdForLectureAsync(lectureId);
            return courseId != null && await CanManageCourseAsync(user, courseId);
        }

        public async Task<bool> CanAccessMaterialAsync(ClaimsPrincipal user, string materialId)
        {
            var courseId = await GetCourseIdForMaterialAsync(materialId);
            return courseId != null && await CanAccessCourseAsync(user, courseId);
        }

        public async Task<bool> CanManageMaterialAsync(ClaimsPrincipal user, string materialId)
        {
            var courseId = await GetCourseIdForMaterialAsync(materialId);
            return courseId != null && await CanManageCourseAsync(user, courseId);
        }

        // Admin: always. Trainer: only courses they own. Student: only when paid (read only).
        private async Task<bool> AuthorizeCourseAsync(ClaimsPrincipal user, string courseId, bool allowPaidStudent)
        {
            if (user.IsInRole(ApplicationRoles.Admin))
                return true;

            if (user.IsInRole(ApplicationRoles.Trainer))
                return await IsCourseOwnerAsync(user, courseId);

            if (allowPaidStudent)
            {
                var studentId = user.FindFirstValue("studentId");
                if (!string.IsNullOrEmpty(studentId))
                    return await _paymentRepository.HasPaidForCourse(studentId, courseId);
            }

            return false;
        }

        private async Task<bool> IsCourseOwnerAsync(ClaimsPrincipal user, string courseId)
        {
            var trainerId = user.FindFirstValue("trainerId");
            if (string.IsNullOrEmpty(trainerId))
                return false;

            return await _context.Courses
                .AnyAsync(c => c.CourseId == courseId && c.TrainerId == trainerId);
        }

        private async Task<string?> GetCourseIdForLectureAsync(string lectureId)
            => await _context.Lectures
                .Where(l => l.LectureId == lectureId)
                .Select(l => l.CourseId)
                .FirstOrDefaultAsync();

        private async Task<string?> GetCourseIdForMaterialAsync(string materialId)
            => await _context.LectureMaterials
                .Where(m => m.MaterialId == materialId)
                .Select(m => m.Lecture != null ? m.Lecture.CourseId : null)
                .FirstOrDefaultAsync();
    }
}

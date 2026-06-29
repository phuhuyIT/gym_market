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
            var lectureId = await _context.LectureMaterials
                .Where(m => m.MaterialId == materialId)
                .Select(m => m.LectureId)
                .FirstOrDefaultAsync();

            return lectureId != null && !(await GetLectureUnlockStateAsync(user, lectureId)).IsLocked;
        }

        public async Task<LectureUnlockState> GetLectureUnlockStateAsync(ClaimsPrincipal user, string lectureId)
        {
            var lecture = await _context.Lectures
                .AsNoTracking()
                .Include(l => l.Module)
                .FirstOrDefaultAsync(l => l.LectureId == lectureId);

            if (lecture == null || string.IsNullOrEmpty(lecture.CourseId))
                return new LectureUnlockState(true, "Lecture not found");

            if (!await CanAccessCourseAsync(user, lecture.CourseId))
                return new LectureUnlockState(true, "Course access is required");

            if (user.IsInRole(ApplicationRoles.Admin) || user.IsInRole(ApplicationRoles.Trainer))
                return new LectureUnlockState(false);

            var studentId = user.FindFirstValue("studentId");
            if (string.IsNullOrEmpty(studentId))
                return new LectureUnlockState(true, "Student access is required");

            var now = DateTime.UtcNow;
            var scheduleLock = CheckSchedule(lecture.Module?.IsPublished == false, "Module is not published")
                ?? CheckDateWindow(lecture.Module?.AvailableFrom, lecture.Module?.AvailableUntil, now, "Module")
                ?? CheckSchedule(!lecture.IsPublished, "Lesson is not published")
                ?? CheckDateWindow(lecture.AvailableFrom, lecture.AvailableUntil, now, "Lesson");

            if (scheduleLock != null)
                return scheduleLock;

            if (!string.IsNullOrEmpty(lecture.Module?.PrerequisiteModuleId))
            {
                var prerequisiteModuleLectureIds = await _context.Lectures
                    .AsNoTracking()
                    .Where(l => l.ModuleId == lecture.Module.PrerequisiteModuleId && l.IsPublished)
                    .Select(l => l.LectureId)
                    .ToListAsync();

                if (prerequisiteModuleLectureIds.Count > 0)
                {
                    var completedPrerequisiteModuleLectures = await _context.LectureProgresses
                        .AsNoTracking()
                        .CountAsync(p => p.StudentId == studentId
                            && p.IsCompleted
                            && prerequisiteModuleLectureIds.Contains(p.LectureId));

                    if (completedPrerequisiteModuleLectures < prerequisiteModuleLectureIds.Count)
                        return new LectureUnlockState(true, "Complete the prerequisite module first");
                }
            }

            if (!string.IsNullOrEmpty(lecture.PrerequisiteLectureId))
            {
                var prerequisiteComplete = await _context.LectureProgresses
                    .AsNoTracking()
                    .AnyAsync(p => p.StudentId == studentId
                        && p.LectureId == lecture.PrerequisiteLectureId
                        && p.IsCompleted);

                if (!prerequisiteComplete)
                    return new LectureUnlockState(true, "Complete the prerequisite lesson first");
            }

            var startedAt = await GetLearnerStartDateAsync(studentId, lecture.CourseId);
            var moduleDrip = BuildDripLock(startedAt, lecture.Module?.UnlockAfterDays, now, "Module");
            if (moduleDrip != null)
                return moduleDrip;

            var lectureDrip = BuildDripLock(startedAt, lecture.UnlockAfterDays, now, "Lesson");
            if (lectureDrip != null)
                return lectureDrip;

            return new LectureUnlockState(false);
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

        private async Task<DateTime?> GetLearnerStartDateAsync(string studentId, string courseId)
        {
            var paidAt = await _context.Payments
                .AsNoTracking()
                .Where(p => p.StudentId == studentId
                    && p.CourseId == courseId
                    && (p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.Completed))
                .OrderBy(p => p.PaymentDate ?? p.CreatedAt)
                .Select(p => p.PaymentDate ?? p.CreatedAt)
                .FirstOrDefaultAsync();

            if (paidAt.HasValue)
                return paidAt.Value;

            return await _context.CourseRegistrations
                .AsNoTracking()
                .Where(r => r.StudentId == studentId
                    && r.CourseId == courseId
                    && (r.PaymentStatus == PaymentStatus.Paid || r.PaymentStatus == PaymentStatus.Completed))
                .OrderBy(r => r.CreatedAt)
                .Select(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }

        private static LectureUnlockState? CheckSchedule(bool isLocked, string reason) =>
            isLocked ? new LectureUnlockState(true, reason) : null;

        private static LectureUnlockState? CheckDateWindow(DateTime? availableFrom, DateTime? availableUntil, DateTime now, string label)
        {
            if (availableFrom.HasValue && availableFrom.Value > now)
                return new LectureUnlockState(true, $"{label} opens later", availableFrom.Value);

            if (availableUntil.HasValue && availableUntil.Value < now)
                return new LectureUnlockState(true, $"{label} is no longer available");

            return null;
        }

        private static LectureUnlockState? BuildDripLock(DateTime? startedAt, int? unlockAfterDays, DateTime now, string label)
        {
            if (!unlockAfterDays.HasValue || unlockAfterDays.Value <= 0)
                return null;

            if (!startedAt.HasValue)
                return new LectureUnlockState(true, "Learning start date is not available");

            var unlocksAt = startedAt.Value.AddDays(unlockAfterDays.Value);
            return unlocksAt > now
                ? new LectureUnlockState(true, $"{label} unlocks later", unlocksAt)
                : null;
        }
    }
}

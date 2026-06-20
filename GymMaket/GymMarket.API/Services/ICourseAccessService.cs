using System.Security.Claims;

namespace GymMarket.API.Services
{
    // Central authority for course-content authorization.
    //
    // "Access" = read/study the content: admins always, the owning trainer, or a student
    // who has paid for the course.
    // "Manage" = create/edit/delete the content: admins always, or the owning trainer only.
    public interface ICourseAccessService
    {
        Task<bool> CanAccessCourseAsync(ClaimsPrincipal user, string courseId);
        Task<bool> CanAccessLectureAsync(ClaimsPrincipal user, string lectureId);
        Task<bool> CanAccessMaterialAsync(ClaimsPrincipal user, string materialId);

        Task<bool> CanManageCourseAsync(ClaimsPrincipal user, string courseId);
        Task<bool> CanManageLectureAsync(ClaimsPrincipal user, string lectureId);
        Task<bool> CanManageMaterialAsync(ClaimsPrincipal user, string materialId);
    }
}

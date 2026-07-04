using System.Security.Claims;
using GymMarket.API.DTOs.CourseStudyGroups;
using GymMarket.API.DTOs.Response;

namespace GymMarket.API.Services;

public interface ICourseStudyGroupService
{
    Task<CourseStudyGroupDto?> EnsureDefaultCohortAsync(string courseId, string actorUserId);

    Task<List<CourseStudyGroupDto>> GetCourseGroupsAsync(string courseId, ClaimsPrincipal user, bool includeAll);

    Task<List<EligibleCourseLearnerDto>> GetEligibleLearnersAsync(string courseId, string? studyGroupId);

    Task<CourseStudyGroupDto?> CreateStudyGroupAsync(string courseId, UpsertCourseStudyGroupDto dto, string actorUserId);

    Task<CourseStudyGroupDto?> UpdateStudyGroupAsync(string studyGroupId, UpsertCourseStudyGroupDto dto, string actorUserId);

    Task<ApiResponse> AddMembersAsync(string studyGroupId, IEnumerable<string> userIds, string actorUserId);

    Task<ApiResponse> RemoveMemberAsync(string studyGroupId, string userId, string actorUserId);

    Task<ApiResponse> UpdateMemberRoleAsync(string studyGroupId, string userId, string role, string actorUserId);

    Task<ApiResponse> ArchiveStudyGroupAsync(string studyGroupId, string actorUserId);
}

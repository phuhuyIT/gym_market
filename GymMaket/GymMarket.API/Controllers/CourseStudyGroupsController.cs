using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.CourseStudyGroups;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CourseStudyGroupsController : ControllerBase
{
    private readonly ICourseAccessService _courseAccessService;
    private readonly ICourseStudyGroupService _studyGroupService;
    private readonly GymMarketContext _context;

    public CourseStudyGroupsController(
        ICourseAccessService courseAccessService,
        ICourseStudyGroupService studyGroupService,
        GymMarketContext context)
    {
        _courseAccessService = courseAccessService;
        _studyGroupService = studyGroupService;
        _context = context;
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetForCourse(string courseId)
    {
        var canManage = await _courseAccessService.CanManageCourseAsync(User, courseId);
        if (!canManage && !await _courseAccessService.CanAccessCourseAsync(User, courseId))
            return Forbid();

        await _studyGroupService.EnsureDefaultCohortAsync(courseId, CurrentUserId());
        var groups = await _studyGroupService.GetCourseGroupsAsync(courseId, User, canManage);
        return Ok(groups);
    }

    [HttpGet("course/{courseId}/manage")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> GetForManagement(string courseId)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        await _studyGroupService.EnsureDefaultCohortAsync(courseId, CurrentUserId());
        var groups = await _studyGroupService.GetCourseGroupsAsync(courseId, User, includeAll: true);
        return Ok(groups);
    }

    [HttpGet("course/{courseId}/eligible-learners")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> GetEligibleLearners(string courseId, [FromQuery] string? studyGroupId)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var learners = await _studyGroupService.GetEligibleLearnersAsync(courseId, studyGroupId);
        return Ok(learners);
    }

    [HttpPost("course/{courseId}/sync")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> SyncDefaultCohort(string courseId)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var cohort = await _studyGroupService.EnsureDefaultCohortAsync(courseId, CurrentUserId());
        return cohort == null
            ? NotFound(new { Message = "COURSE_NOT_FOUND" })
            : Ok(cohort);
    }

    [HttpPost("course/{courseId}/groups")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> CreateStudyGroup(string courseId, UpsertCourseStudyGroupDto dto)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();
        if (!await _context.Courses.AsNoTracking().AnyAsync(c => c.CourseId == courseId))
            return NotFound(new { Message = "COURSE_NOT_FOUND" });

        var group = await _studyGroupService.CreateStudyGroupAsync(courseId, dto, CurrentUserId());
        return group == null
            ? BadRequest(new { Message = "STUDY_GROUP_NAME_REQUIRED" })
            : Ok(group);
    }

    [HttpPut("groups/{studyGroupId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> UpdateStudyGroup(string studyGroupId, UpsertCourseStudyGroupDto dto)
    {
        var courseId = await GetStudyGroupCourseId(studyGroupId);
        if (courseId == null)
            return NotFound(new { Message = "STUDY_GROUP_NOT_FOUND" });

        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var group = await _studyGroupService.UpdateStudyGroupAsync(studyGroupId, dto, CurrentUserId());
        return group == null
            ? BadRequest(new { Message = "STUDY_GROUP_NAME_REQUIRED" })
            : Ok(group);
    }

    [HttpPost("groups/{studyGroupId}/members")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> AddMembers(string studyGroupId, ManageCourseStudyGroupMembersDto dto)
    {
        if (!await CanManageStudyGroup(studyGroupId))
            return Forbid();

        var result = await _studyGroupService.AddMembersAsync(studyGroupId, dto.UserIds, CurrentUserId());
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("groups/{studyGroupId}/members/{userId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> RemoveMember(string studyGroupId, string userId)
    {
        if (!await CanManageStudyGroup(studyGroupId))
            return Forbid();

        var result = await _studyGroupService.RemoveMemberAsync(studyGroupId, userId, CurrentUserId());
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("groups/{studyGroupId}/members/{userId}/role")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> UpdateMemberRole(string studyGroupId, string userId, UpdateCourseStudyGroupMemberRoleDto dto)
    {
        if (!await CanManageStudyGroup(studyGroupId))
            return Forbid();

        var result = await _studyGroupService.UpdateMemberRoleAsync(studyGroupId, userId, dto.Role, CurrentUserId());
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("groups/{studyGroupId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> ArchiveStudyGroup(string studyGroupId)
    {
        if (!await CanManageStudyGroup(studyGroupId))
            return Forbid();

        var result = await _studyGroupService.ArchiveStudyGroupAsync(studyGroupId, CurrentUserId());
        return StatusCode(result.StatusCode, result);
    }

    private async Task<bool> CanManageStudyGroup(string studyGroupId)
    {
        var courseId = await GetStudyGroupCourseId(studyGroupId);
        return courseId != null && await _courseAccessService.CanManageCourseAsync(User, courseId);
    }

    private async Task<string?> GetStudyGroupCourseId(string studyGroupId)
        => await _context.CourseStudyGroups
            .AsNoTracking()
            .Where(g => g.StudyGroupId == studyGroupId)
            .Select(g => g.CourseId)
            .FirstOrDefaultAsync();

    private string CurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
}

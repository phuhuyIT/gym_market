using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Assignments;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly GymMarketContext _context;
    private readonly ICourseAccessService _courseAccessService;

    public AssignmentsController(GymMarketContext context, ICourseAccessService courseAccessService)
    {
        _context = context;
        _courseAccessService = courseAccessService;
    }

    [HttpGet("course/{courseId}/manage")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> GetForManagement(string courseId)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var assignments = await AssignmentQuery()
            .Where(a => a.CourseId == courseId)
            .OrderBy(a => a.DueAt ?? DateTime.MaxValue)
            .ThenBy(a => a.Title)
            .ToListAsync();

        return Ok(assignments.Select(a => ToAssignmentDto(a)));
    }

    [HttpGet("course/{courseId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetForStudent(string courseId)
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

        if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
            return Forbid();

        var assignments = await AssignmentQuery()
            .Where(a => a.CourseId == courseId && a.Status == AssignmentStatus.Published)
            .OrderBy(a => a.DueAt ?? DateTime.MaxValue)
            .ThenBy(a => a.Title)
            .ToListAsync();

        return Ok(assignments.Select(a => ToAssignmentDto(a, studentId)));
    }

    [HttpPost("course/{courseId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> Create(string courseId, UpsertCourseAssignmentDto dto)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var validationError = await ValidateAssignment(courseId, dto);
        if (validationError != null)
            return BadRequest(new { Message = validationError });

        var now = DateTime.UtcNow;
        var assignment = new CourseAssignment
        {
            AssignmentId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            CreatedAt = now
        };

        ApplyAssignment(assignment, dto, now);
        _context.CourseAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        assignment = await AssignmentQuery().FirstAsync(a => a.AssignmentId == assignment.AssignmentId);
        return Ok(ToAssignmentDto(assignment));
    }

    [HttpPut("{assignmentId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> Update(string assignmentId, UpsertCourseAssignmentDto dto)
    {
        var assignment = await _context.CourseAssignments
            .Include(a => a.Submissions)
            .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
        if (assignment == null)
            return NotFound(new { Message = "ASSIGNMENT_NOT_FOUND" });

        if (!await _courseAccessService.CanManageCourseAsync(User, assignment.CourseId))
            return Forbid();

        var validationError = await ValidateAssignment(assignment.CourseId, dto);
        if (validationError != null)
            return BadRequest(new { Message = validationError });

        ApplyAssignment(assignment, dto, DateTime.UtcNow);
        await _context.SaveChangesAsync();

        assignment = await AssignmentQuery().FirstAsync(a => a.AssignmentId == assignmentId);
        return Ok(ToAssignmentDto(assignment));
    }

    [HttpDelete("{assignmentId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> Delete(string assignmentId)
    {
        var assignment = await _context.CourseAssignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
        if (assignment == null)
            return NotFound(new { Message = "ASSIGNMENT_NOT_FOUND" });

        if (!await _courseAccessService.CanManageCourseAsync(User, assignment.CourseId))
            return Forbid();

        _context.CourseAssignments.Remove(assignment);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{assignmentId}/submissions")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> GetSubmissions(string assignmentId)
    {
        var assignment = await _context.CourseAssignments.AsNoTracking().FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
        if (assignment == null)
            return NotFound(new { Message = "ASSIGNMENT_NOT_FOUND" });

        if (!await _courseAccessService.CanManageCourseAsync(User, assignment.CourseId))
            return Forbid();

        var submissions = await _context.AssignmentSubmissions
            .AsNoTracking()
            .Include(s => s.Student)
            .Where(s => s.AssignmentId == assignmentId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();

        return Ok(submissions.Select(ToSubmissionDto));
    }

    [HttpPost("{assignmentId}/submit")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Submit(string assignmentId, SubmitAssignmentDto dto)
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

        var assignment = await _context.CourseAssignments.FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
        if (assignment == null)
            return NotFound(new { Message = "ASSIGNMENT_NOT_FOUND" });

        if (assignment.Status != AssignmentStatus.Published)
            return Conflict(new { Message = "ASSIGNMENT_NOT_OPEN" });

        if (!await _courseAccessService.CanAccessCourseAsync(User, assignment.CourseId))
            return Forbid();

        if (string.IsNullOrWhiteSpace(dto.TextResponse) && string.IsNullOrWhiteSpace(dto.AttachmentUrl))
            return BadRequest(new { Message = "SUBMISSION_CONTENT_REQUIRED" });

        var now = DateTime.UtcNow;
        var submission = await _context.AssignmentSubmissions
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

        if (submission == null)
        {
            submission = new AssignmentSubmission
            {
                SubmissionId = Guid.NewGuid().ToString(),
                AssignmentId = assignmentId,
                StudentId = studentId,
                SubmittedAt = now
            };
            _context.AssignmentSubmissions.Add(submission);
        }

        submission.TextResponse = dto.TextResponse?.Trim();
        submission.AttachmentUrl = dto.AttachmentUrl?.Trim();
        submission.Status = AssignmentSubmissionStatus.Submitted;
        submission.Score = null;
        submission.ScorePercent = null;
        submission.Feedback = null;
        submission.GradedAt = null;
        submission.UpdatedAt = now;
        submission.SubmittedAt = now;

        await _context.SaveChangesAsync();

        submission = await _context.AssignmentSubmissions
            .AsNoTracking()
            .Include(s => s.Student)
            .FirstAsync(s => s.SubmissionId == submission.SubmissionId);
        return Ok(ToSubmissionDto(submission));
    }

    [HttpPut("submissions/{submissionId}/grade")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> Grade(string submissionId, GradeAssignmentSubmissionDto dto)
    {
        var submission = await _context.AssignmentSubmissions
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);
        if (submission?.Assignment == null)
            return NotFound(new { Message = "SUBMISSION_NOT_FOUND" });

        if (!await _courseAccessService.CanManageCourseAsync(User, submission.Assignment.CourseId))
            return Forbid();

        if (dto.Score < 0 || dto.Score > submission.Assignment.PointsPossible)
            return BadRequest(new { Message = "ASSIGNMENT_SCORE_INVALID" });

        submission.Score = dto.Score;
        submission.ScorePercent = submission.Assignment.PointsPossible <= 0
            ? 0
            : Math.Round(dto.Score / submission.Assignment.PointsPossible * 100m, 2, MidpointRounding.AwayFromZero);
        submission.Feedback = dto.Feedback?.Trim();
        submission.Status = AssignmentSubmissionStatus.Graded;
        submission.GradedAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(ToSubmissionDto(submission));
    }

    private IQueryable<CourseAssignment> AssignmentQuery() => _context.CourseAssignments
        .AsNoTracking()
        .Include(a => a.GradeCategory)
        .Include(a => a.Submissions).ThenInclude(s => s.Student);

    private async Task<string?> ValidateAssignment(string courseId, UpsertCourseAssignmentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return "ASSIGNMENT_TITLE_REQUIRED";

        if (dto.PointsPossible <= 0)
            return "ASSIGNMENT_POINTS_INVALID";

        if (!string.IsNullOrWhiteSpace(dto.GradeCategoryId))
        {
            var categoryExists = await _context.GradeCategories
                .AnyAsync(c => c.CourseId == courseId && c.CategoryId == dto.GradeCategoryId);
            if (!categoryExists)
                return "GRADEBOOK_CATEGORY_NOT_FOUND";
        }

        return null;
    }

    private static void ApplyAssignment(CourseAssignment assignment, UpsertCourseAssignmentDto dto, DateTime now)
    {
        assignment.Title = dto.Title.Trim();
        assignment.Instructions = dto.Instructions?.Trim();
        assignment.GradeCategoryId = string.IsNullOrWhiteSpace(dto.GradeCategoryId) ? null : dto.GradeCategoryId.Trim();
        assignment.PointsPossible = dto.PointsPossible;
        assignment.DueAt = dto.DueAt;
        assignment.SubmissionType = AssignmentSubmissionType.Normalize(dto.SubmissionType);
        assignment.Status = AssignmentStatus.Normalize(dto.Status);
        assignment.UpdatedAt = now;
    }

    private static CourseAssignmentDto ToAssignmentDto(CourseAssignment assignment, string? studentId = null)
    {
        var submissions = assignment.Submissions.ToList();
        return new CourseAssignmentDto
        {
            AssignmentId = assignment.AssignmentId,
            CourseId = assignment.CourseId,
            GradeCategoryId = assignment.GradeCategoryId,
            GradeCategoryName = assignment.GradeCategory?.Name,
            Title = assignment.Title,
            Instructions = assignment.Instructions,
            PointsPossible = assignment.PointsPossible,
            DueAt = assignment.DueAt,
            SubmissionType = assignment.SubmissionType,
            Status = assignment.Status,
            SubmissionCount = submissions.Count,
            GradedCount = submissions.Count(s => s.Status == AssignmentSubmissionStatus.Graded),
            MySubmission = studentId == null
                ? null
                : submissions.Where(s => s.StudentId == studentId)
                    .OrderByDescending(s => s.SubmittedAt)
                    .Select(ToSubmissionDto)
                    .FirstOrDefault()
        };
    }

    private static AssignmentSubmissionDto ToSubmissionDto(AssignmentSubmission submission) => new()
    {
        SubmissionId = submission.SubmissionId,
        AssignmentId = submission.AssignmentId,
        StudentId = submission.StudentId,
        StudentName = submission.Student?.Name,
        StudentEmail = submission.Student?.Email,
        TextResponse = submission.TextResponse,
        AttachmentUrl = submission.AttachmentUrl,
        Score = submission.Score,
        ScorePercent = submission.ScorePercent,
        Status = submission.Status,
        Feedback = submission.Feedback,
        SubmittedAt = submission.SubmittedAt,
        GradedAt = submission.GradedAt,
        UpdatedAt = submission.UpdatedAt
    };

    private string CurrentStudentId()
    {
        return User.FindFirstValue("studentId") ?? string.Empty;
    }
}

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
            .Include(a => a.RubricCriteria)
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
            .Include(s => s.RubricScores).ThenInclude(r => r.Criterion)
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
            .Include(s => s.RubricScores)
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
        if (submission.RubricScores.Count > 0)
            _context.AssignmentRubricScores.RemoveRange(submission.RubricScores);

        await _context.SaveChangesAsync();

        submission = await _context.AssignmentSubmissions
            .AsNoTracking()
            .Include(s => s.Student)
            .Include(s => s.RubricScores).ThenInclude(r => r.Criterion)
            .FirstAsync(s => s.SubmissionId == submission.SubmissionId);
        return Ok(ToSubmissionDto(submission));
    }

    [HttpPut("submissions/{submissionId}/grade")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> Grade(string submissionId, GradeAssignmentSubmissionDto dto)
    {
        var submission = await _context.AssignmentSubmissions
            .Include(s => s.Assignment).ThenInclude(a => a!.RubricCriteria)
            .Include(s => s.Student)
            .Include(s => s.RubricScores).ThenInclude(r => r.Criterion)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);
        if (submission?.Assignment == null)
            return NotFound(new { Message = "SUBMISSION_NOT_FOUND" });

        if (!await _courseAccessService.CanManageCourseAsync(User, submission.Assignment.CourseId))
            return Forbid();

        var criteria = submission.Assignment.RubricCriteria.OrderBy(c => c.Order).ToList();
        if (criteria.Count > 0)
        {
            var validationError = ValidateRubricScores(criteria, dto.RubricScores);
            if (validationError != null)
                return BadRequest(new { Message = validationError });

            if (submission.RubricScores.Count > 0)
                _context.AssignmentRubricScores.RemoveRange(submission.RubricScores);

            var now = DateTime.UtcNow;
            var scoreByCriterion = dto.RubricScores.ToDictionary(s => s.CriterionId, StringComparer.Ordinal);
            foreach (var criterion in criteria)
            {
                var rubricScore = scoreByCriterion[criterion.CriterionId];
                _context.AssignmentRubricScores.Add(new AssignmentRubricScore
                {
                    RubricScoreId = Guid.NewGuid().ToString(),
                    SubmissionId = submission.SubmissionId,
                    CriterionId = criterion.CriterionId,
                    Score = rubricScore.Score,
                    Feedback = rubricScore.Feedback?.Trim(),
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }

            submission.Score = dto.RubricScores.Sum(s => s.Score);
        }
        else
        {
            if (!dto.Score.HasValue || dto.Score < 0 || dto.Score > submission.Assignment.PointsPossible)
                return BadRequest(new { Message = "ASSIGNMENT_SCORE_INVALID" });

            submission.Score = dto.Score.Value;
        }

        submission.ScorePercent = submission.Assignment.PointsPossible <= 0
            ? 0
            : Math.Round(submission.Score.Value / submission.Assignment.PointsPossible * 100m, 2, MidpointRounding.AwayFromZero);
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
        .Include(a => a.RubricCriteria)
        .Include(a => a.Submissions).ThenInclude(s => s.Student)
        .Include(a => a.Submissions).ThenInclude(s => s.RubricScores).ThenInclude(r => r.Criterion);

    private async Task<string?> ValidateAssignment(string courseId, UpsertCourseAssignmentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return "ASSIGNMENT_TITLE_REQUIRED";

        if (dto.PointsPossible <= 0)
            return "ASSIGNMENT_POINTS_INVALID";

        if (dto.RubricCriteria.Count > 0)
        {
            if (dto.RubricCriteria.Any(c => string.IsNullOrWhiteSpace(c.Title)))
                return "RUBRIC_CRITERION_TITLE_REQUIRED";

            if (dto.RubricCriteria.Any(c => c.PointsPossible <= 0))
                return "RUBRIC_CRITERION_POINTS_INVALID";

            var duplicateTitles = dto.RubricCriteria
                .GroupBy(c => c.Title.Trim(), StringComparer.OrdinalIgnoreCase)
                .Any(g => g.Count() > 1);
            if (duplicateTitles)
                return "RUBRIC_CRITERION_TITLE_DUPLICATE";
        }

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
        assignment.PointsPossible = dto.RubricCriteria.Count > 0
            ? dto.RubricCriteria.Sum(c => c.PointsPossible)
            : dto.PointsPossible;
        assignment.DueAt = dto.DueAt;
        assignment.SubmissionType = AssignmentSubmissionType.Normalize(dto.SubmissionType);
        assignment.Status = AssignmentStatus.Normalize(dto.Status);
        assignment.UpdatedAt = now;
        ApplyRubricCriteria(assignment, dto.RubricCriteria, now);
    }

    private static void ApplyRubricCriteria(CourseAssignment assignment, List<UpsertAssignmentRubricCriterionDto> criteria, DateTime now)
    {
        var normalized = criteria
            .OrderBy(c => c.Order <= 0 ? int.MaxValue : c.Order)
            .Select((criterion, index) => new
            {
                CriterionId = string.IsNullOrWhiteSpace(criterion.CriterionId) ? Guid.NewGuid().ToString() : criterion.CriterionId.Trim(),
                Title = criterion.Title.Trim(),
                Description = criterion.Description?.Trim(),
                criterion.PointsPossible,
                Order = index + 1
            })
            .ToList();

        var requestedIds = normalized.Select(c => c.CriterionId).ToHashSet(StringComparer.Ordinal);
        foreach (var removed in assignment.RubricCriteria.Where(c => !requestedIds.Contains(c.CriterionId)).ToList())
            assignment.RubricCriteria.Remove(removed);

        foreach (var criterion in normalized)
        {
            var entity = assignment.RubricCriteria.FirstOrDefault(c => c.CriterionId == criterion.CriterionId);
            if (entity == null)
            {
                entity = new AssignmentRubricCriterion
                {
                    CriterionId = criterion.CriterionId,
                    AssignmentId = assignment.AssignmentId,
                    CreatedAt = now
                };
                assignment.RubricCriteria.Add(entity);
            }

            entity.Title = criterion.Title;
            entity.Description = criterion.Description;
            entity.PointsPossible = criterion.PointsPossible;
            entity.Order = criterion.Order;
            entity.UpdatedAt = now;
        }
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
            RubricCriteria = assignment.RubricCriteria.OrderBy(c => c.Order).Select(ToRubricCriterionDto).ToList(),
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
        UpdatedAt = submission.UpdatedAt,
        RubricScores = submission.RubricScores
            .OrderBy(s => s.Criterion?.Order ?? int.MaxValue)
            .Select(ToRubricScoreDto)
            .ToList()
    };

    private static AssignmentRubricCriterionDto ToRubricCriterionDto(AssignmentRubricCriterion criterion) => new()
    {
        CriterionId = criterion.CriterionId,
        AssignmentId = criterion.AssignmentId,
        Title = criterion.Title,
        Description = criterion.Description,
        PointsPossible = criterion.PointsPossible,
        Order = criterion.Order
    };

    private static AssignmentRubricScoreDto ToRubricScoreDto(AssignmentRubricScore score) => new()
    {
        RubricScoreId = score.RubricScoreId,
        SubmissionId = score.SubmissionId,
        CriterionId = score.CriterionId,
        CriterionTitle = score.Criterion?.Title,
        PointsPossible = score.Criterion?.PointsPossible ?? 0,
        Score = score.Score,
        Feedback = score.Feedback
    };

    private static string? ValidateRubricScores(List<AssignmentRubricCriterion> criteria, List<GradeAssignmentRubricScoreDto> scores)
    {
        if (scores.Count != criteria.Count)
            return "RUBRIC_SCORE_REQUIRED";

        var criteriaById = criteria.ToDictionary(c => c.CriterionId, StringComparer.Ordinal);
        var duplicateScore = scores.GroupBy(s => s.CriterionId, StringComparer.Ordinal).Any(g => g.Count() > 1);
        if (duplicateScore)
            return "RUBRIC_SCORE_DUPLICATE";

        foreach (var score in scores)
        {
            if (!criteriaById.TryGetValue(score.CriterionId, out var criterion))
                return "RUBRIC_CRITERION_NOT_FOUND";

            if (score.Score < 0 || score.Score > criterion.PointsPossible)
                return "RUBRIC_SCORE_INVALID";
        }

        return null;
    }

    private string CurrentStudentId()
    {
        return User.FindFirstValue("studentId") ?? string.Empty;
    }
}

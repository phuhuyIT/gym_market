using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Discussions;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CourseDiscussionsController : ControllerBase
{
    private readonly GymMarketContext _context;
    private readonly ICourseAccessService _courseAccessService;
    private readonly UserManager<AppUser> _userManager;

    public CourseDiscussionsController(
        GymMarketContext context,
        ICourseAccessService courseAccessService,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _courseAccessService = courseAccessService;
        _userManager = userManager;
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetCourseQuestions(string courseId, [FromQuery] string? status, [FromQuery] string? search)
    {
        if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
            return Forbid();

        var query = QuestionQuery()
            .Where(q => q.CourseId == courseId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = DiscussionQuestionStatus.Normalize(status);
            query = query.Where(q => q.Status == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(q => q.Title.Contains(term) || q.Body.Contains(term));
        }

        var questions = await query
            .OrderByDescending(q => q.IsPinned)
            .ThenByDescending(q => q.LastActivityAt)
            .ToListAsync();

        return Ok(questions.Select(q => ToQuestionDto(q, includeAnswers: false)));
    }

    [HttpGet("questions/{questionId}")]
    public async Task<IActionResult> GetQuestion(string questionId)
    {
        var question = await QuestionQuery()
            .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        if (question == null)
            return NotFound(new { Message = "DISCUSSION_QUESTION_NOT_FOUND" });

        if (!await _courseAccessService.CanAccessCourseAsync(User, question.CourseId))
            return Forbid();

        return Ok(ToQuestionDto(question, includeAnswers: true));
    }

    [HttpPost("course/{courseId}/questions")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateQuestion(string courseId, CreateDiscussionQuestionDto dto)
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

        if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
            return Forbid();

        var validationError = ValidateQuestion(dto);
        if (validationError != null)
            return BadRequest(new { Message = validationError });

        var now = DateTime.UtcNow;
        var question = new CourseDiscussionQuestion
        {
            QuestionId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            StudentId = studentId,
            Title = dto.Title.Trim(),
            Body = dto.Body.Trim(),
            Status = DiscussionQuestionStatus.Open,
            CreatedAt = now,
            UpdatedAt = now,
            LastActivityAt = now
        };

        _context.CourseDiscussionQuestions.Add(question);
        await _context.SaveChangesAsync();

        question = await QuestionQuery().FirstAsync(q => q.QuestionId == question.QuestionId);
        return Ok(ToQuestionDto(question, includeAnswers: true));
    }

    [HttpPost("questions/{questionId}/answers")]
    public async Task<IActionResult> CreateAnswer(string questionId, CreateDiscussionAnswerDto dto)
    {
        var question = await _context.CourseDiscussionQuestions
            .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        if (question == null)
            return NotFound(new { Message = "DISCUSSION_QUESTION_NOT_FOUND" });

        if (!await _courseAccessService.CanAccessCourseAsync(User, question.CourseId))
            return Forbid();

        if (question.Status == DiscussionQuestionStatus.Closed)
            return Conflict(new { Message = "DISCUSSION_QUESTION_CLOSED" });

        if (string.IsNullOrWhiteSpace(dto.Body))
            return BadRequest(new { Message = "DISCUSSION_ANSWER_BODY_REQUIRED" });

        var author = await GetAuthorAsync();
        if (author == null)
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "DISCUSSION_AUTHOR_NOT_FOUND" });

        var now = DateTime.UtcNow;
        var answer = new CourseDiscussionAnswer
        {
            AnswerId = Guid.NewGuid().ToString(),
            QuestionId = questionId,
            AuthorUserId = author.UserId,
            AuthorEntityId = author.EntityId,
            AuthorRole = author.Role,
            AuthorName = author.Name,
            AuthorEmail = author.Email,
            Body = dto.Body.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        question.UpdatedAt = now;
        question.LastActivityAt = now;

        _context.CourseDiscussionAnswers.Add(answer);
        await _context.SaveChangesAsync();

        answer = await _context.CourseDiscussionAnswers
            .AsNoTracking()
            .FirstAsync(a => a.AnswerId == answer.AnswerId);
        return Ok(ToAnswerDto(answer));
    }

    [HttpPut("questions/{questionId}/accept/{answerId}")]
    public async Task<IActionResult> AcceptAnswer(string questionId, string answerId)
    {
        var question = await _context.CourseDiscussionQuestions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        if (question == null)
            return NotFound(new { Message = "DISCUSSION_QUESTION_NOT_FOUND" });

        if (!await _courseAccessService.CanAccessCourseAsync(User, question.CourseId))
            return Forbid();

        if (!CanAccept(question))
            return Forbid();

        var answer = question.Answers.FirstOrDefault(a => a.AnswerId == answerId);
        if (answer == null)
            return NotFound(new { Message = "DISCUSSION_ANSWER_NOT_FOUND" });

        foreach (var existing in question.Answers)
        {
            existing.IsAccepted = existing.AnswerId == answerId;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        var now = DateTime.UtcNow;
        question.AcceptedAnswerId = answerId;
        question.Status = DiscussionQuestionStatus.Answered;
        question.UpdatedAt = now;
        question.LastActivityAt = now;

        await _context.SaveChangesAsync();
        question = await QuestionQuery().FirstAsync(q => q.QuestionId == questionId);
        return Ok(ToQuestionDto(question, includeAnswers: true));
    }

    [HttpPut("questions/{questionId}/moderation")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> ModerateQuestion(string questionId, ModerateDiscussionQuestionDto dto)
    {
        var question = await _context.CourseDiscussionQuestions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        if (question == null)
            return NotFound(new { Message = "DISCUSSION_QUESTION_NOT_FOUND" });

        if (!await _courseAccessService.CanManageCourseAsync(User, question.CourseId))
            return Forbid();

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            var status = DiscussionQuestionStatus.Normalize(dto.Status);
            question.Status = status;
            if (status != DiscussionQuestionStatus.Answered)
            {
                question.AcceptedAnswerId = null;
                foreach (var answer in question.Answers)
                    answer.IsAccepted = false;
            }
        }

        if (dto.IsPinned.HasValue)
            question.IsPinned = dto.IsPinned.Value;

        question.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        question = await QuestionQuery().FirstAsync(q => q.QuestionId == questionId);
        return Ok(ToQuestionDto(question, includeAnswers: true));
    }

    [HttpDelete("questions/{questionId}")]
    public async Task<IActionResult> DeleteQuestion(string questionId)
    {
        var question = await _context.CourseDiscussionQuestions.FirstOrDefaultAsync(q => q.QuestionId == questionId);
        if (question == null)
            return NotFound(new { Message = "DISCUSSION_QUESTION_NOT_FOUND" });

        if (!await _courseAccessService.CanAccessCourseAsync(User, question.CourseId))
            return Forbid();

        if (!CanDeleteQuestion(question))
            return Forbid();

        _context.CourseDiscussionQuestions.Remove(question);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("answers/{answerId}")]
    public async Task<IActionResult> DeleteAnswer(string answerId)
    {
        var answer = await _context.CourseDiscussionAnswers
            .Include(a => a.Question)
            .FirstOrDefaultAsync(a => a.AnswerId == answerId);
        if (answer?.Question == null)
            return NotFound(new { Message = "DISCUSSION_ANSWER_NOT_FOUND" });

        if (!await _courseAccessService.CanAccessCourseAsync(User, answer.Question.CourseId))
            return Forbid();

        if (!CanDeleteAnswer(answer))
            return Forbid();

        if (answer.IsAccepted || answer.Question.AcceptedAnswerId == answer.AnswerId)
        {
            answer.Question.AcceptedAnswerId = null;
            answer.Question.Status = DiscussionQuestionStatus.Open;
        }

        answer.Question.UpdatedAt = DateTime.UtcNow;
        answer.Question.LastActivityAt = DateTime.UtcNow;
        _context.CourseDiscussionAnswers.Remove(answer);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<CourseDiscussionQuestion> QuestionQuery() => _context.CourseDiscussionQuestions
        .AsNoTracking()
        .Include(q => q.Student)
        .Include(q => q.Answers);

    private static string? ValidateQuestion(CreateDiscussionQuestionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return "DISCUSSION_TITLE_REQUIRED";

        if (string.IsNullOrWhiteSpace(dto.Body))
            return "DISCUSSION_BODY_REQUIRED";

        return null;
    }

    private DiscussionQuestionDto ToQuestionDto(CourseDiscussionQuestion question, bool includeAnswers)
    {
        var answers = question.Answers
            .OrderBy(a => a.CreatedAt)
            .Select(ToAnswerDto)
            .ToList();

        return new DiscussionQuestionDto
        {
            QuestionId = question.QuestionId,
            CourseId = question.CourseId,
            StudentId = question.StudentId,
            StudentName = question.Student?.Name,
            StudentEmail = question.Student?.Email,
            Title = question.Title,
            Body = question.Body,
            Status = question.Status,
            IsPinned = question.IsPinned,
            AcceptedAnswerId = question.AcceptedAnswerId,
            AnswerCount = answers.Count,
            CanAccept = CanAccept(question),
            CreatedAt = question.CreatedAt,
            UpdatedAt = question.UpdatedAt,
            LastActivityAt = question.LastActivityAt,
            Answers = includeAnswers ? answers : []
        };
    }

    private DiscussionAnswerDto ToAnswerDto(CourseDiscussionAnswer answer) => new()
    {
        AnswerId = answer.AnswerId,
        QuestionId = answer.QuestionId,
        AuthorUserId = answer.AuthorUserId,
        AuthorEntityId = answer.AuthorEntityId,
        AuthorRole = answer.AuthorRole,
        AuthorName = answer.AuthorName,
        AuthorEmail = answer.AuthorEmail,
        Body = answer.Body,
        IsAccepted = answer.IsAccepted,
        CanDelete = CanDeleteAnswer(answer),
        CreatedAt = answer.CreatedAt,
        UpdatedAt = answer.UpdatedAt
    };

    private async Task<DiscussionAuthor?> GetAuthorAsync()
    {
        var userId = CurrentUserId();

        if (User.IsInRole(DiscussionAuthorRole.Admin))
        {
            var user = string.IsNullOrWhiteSpace(userId) ? null : await _userManager.FindByIdAsync(userId);
            return new DiscussionAuthor(
                userId,
                userId,
                DiscussionAuthorRole.Admin,
                user?.FullName ?? user?.Email ?? "Admin",
                user?.Email);
        }

        if (User.IsInRole(DiscussionAuthorRole.Trainer))
        {
            var trainerId = User.FindFirstValue("trainerId");
            if (string.IsNullOrWhiteSpace(trainerId))
                return null;

            var trainer = await _context.Trainers.AsNoTracking().FirstOrDefaultAsync(t => t.TrainerId == trainerId);
            if (trainer == null)
                return null;

            return new DiscussionAuthor(
                userId,
                trainer.TrainerId,
                DiscussionAuthorRole.Trainer,
                trainer.Name ?? trainer.Email ?? "Trainer",
                trainer.Email);
        }

        var studentId = CurrentStudentId();
        if (string.IsNullOrWhiteSpace(studentId))
            return null;

        var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null)
            return null;

        return new DiscussionAuthor(
            userId,
            student.StudentId,
            DiscussionAuthorRole.Student,
            student.Name ?? student.Email ?? "Student",
            student.Email);
    }

    private bool CanAccept(CourseDiscussionQuestion question)
    {
        return User.IsInRole(DiscussionAuthorRole.Admin)
            || User.IsInRole(DiscussionAuthorRole.Trainer)
            || question.StudentId == CurrentStudentId();
    }

    private bool CanDeleteQuestion(CourseDiscussionQuestion question)
    {
        return User.IsInRole(DiscussionAuthorRole.Admin)
            || User.IsInRole(DiscussionAuthorRole.Trainer)
            || question.StudentId == CurrentStudentId();
    }

    private bool CanDeleteAnswer(CourseDiscussionAnswer answer)
    {
        return User.IsInRole(DiscussionAuthorRole.Admin)
            || User.IsInRole(DiscussionAuthorRole.Trainer)
            || (!string.IsNullOrWhiteSpace(answer.AuthorEntityId) && answer.AuthorEntityId == CurrentStudentId());
    }

    private string CurrentStudentId()
    {
        return User.FindFirstValue("studentId") ?? string.Empty;
    }

    private string? CurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private sealed record DiscussionAuthor(
        string? UserId,
        string? EntityId,
        string Role,
        string Name,
        string? Email);
}

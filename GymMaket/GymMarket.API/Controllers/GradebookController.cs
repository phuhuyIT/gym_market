using System.Security.Claims;
using System.Text;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Gradebook;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GradebookController : ControllerBase
{
    private const string DefaultCategoryName = "Assessments";
    private readonly GymMarketContext _context;
    private readonly ICourseAccessService _courseAccessService;

    public GradebookController(GymMarketContext context, ICourseAccessService courseAccessService)
    {
        _context = context;
        _courseAccessService = courseAccessService;
    }

    [HttpGet("course/{courseId}/policy")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> GetPolicy(string courseId)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course == null)
            return NotFound(new { Message = "COURSE_NOT_FOUND" });

        var categories = await EnsureCategoriesAsync(courseId);
        var items = await LoadGradeItemsAsync(courseId, categories);

        return Ok(new GradebookPolicyDto
        {
            CourseId = course.CourseId,
            CourseTitle = course.Title,
            Categories = categories.Select(ToCategoryDto).ToList(),
            Items = items
        });
    }

    [HttpPut("course/{courseId}/policy")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> UpdatePolicy(string courseId, UpdateGradebookPolicyDto dto)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        if (!await _context.Courses.AnyAsync(c => c.CourseId == courseId))
            return NotFound(new { Message = "COURSE_NOT_FOUND" });

        var validationError = ValidatePolicy(dto);
        if (validationError != null)
            return BadRequest(new { Message = validationError });

        var now = DateTime.UtcNow;
        var existing = await _context.GradeCategories
            .Where(c => c.CourseId == courseId)
            .ToListAsync();

        var normalizedCategories = dto.Categories
            .OrderBy(c => c.Order)
            .Select((category, index) => new
            {
                CategoryId = string.IsNullOrWhiteSpace(category.CategoryId) ? Guid.NewGuid().ToString() : category.CategoryId.Trim(),
                Name = category.Name.Trim(),
                category.WeightPercent,
                Order = category.Order <= 0 ? index + 1 : category.Order
            })
            .ToList();

        var requestedIds = normalizedCategories.Select(c => c.CategoryId).ToHashSet(StringComparer.Ordinal);
        var removedCategories = existing.Where(c => !requestedIds.Contains(c.CategoryId)).ToList();
        if (removedCategories.Count > 0)
        {
            var removedIds = removedCategories.Select(c => c.CategoryId).ToList();
            var affectedQuizzes = await _context.CourseQuizzes
                .Where(q => q.CourseId == courseId && q.GradeCategoryId != null && removedIds.Contains(q.GradeCategoryId))
                .ToListAsync();
            foreach (var quiz in affectedQuizzes)
                quiz.GradeCategoryId = null;

            var affectedAssignments = await _context.CourseAssignments
                .Where(a => a.CourseId == courseId && a.GradeCategoryId != null && removedIds.Contains(a.GradeCategoryId))
                .ToListAsync();
            foreach (var assignment in affectedAssignments)
                assignment.GradeCategoryId = null;

            _context.GradeCategories.RemoveRange(removedCategories);
        }

        foreach (var category in normalizedCategories)
        {
            var entity = existing.FirstOrDefault(c => c.CategoryId == category.CategoryId);
            if (entity == null)
            {
                entity = new GradeCategory
                {
                    CategoryId = category.CategoryId,
                    CourseId = courseId,
                    CreatedAt = now
                };
                _context.GradeCategories.Add(entity);
            }

            entity.Name = category.Name;
            entity.WeightPercent = category.WeightPercent;
            entity.Order = category.Order;
            entity.IsDefault = category.Order == normalizedCategories.Min(c => c.Order);
            entity.UpdatedAt = now;
        }

        var validCategoryIds = normalizedCategories.Select(c => c.CategoryId).ToHashSet(StringComparer.Ordinal);
        var itemAssignments = dto.Items
            .Where(i => !string.IsNullOrWhiteSpace(i.ItemId))
            .ToDictionary(i => i.ItemId, i => i.CategoryId, StringComparer.Ordinal);

        var quizzes = await _context.CourseQuizzes
            .Where(q => q.CourseId == courseId)
            .ToListAsync();

        foreach (var quiz in quizzes)
        {
            if (!itemAssignments.TryGetValue(quiz.QuizId, out var categoryId))
                continue;

            quiz.GradeCategoryId = !string.IsNullOrWhiteSpace(categoryId) && validCategoryIds.Contains(categoryId)
                ? categoryId
                : null;
        }

        var assignments = await _context.CourseAssignments
            .Where(a => a.CourseId == courseId)
            .ToListAsync();

        foreach (var assignment in assignments)
        {
            if (!itemAssignments.TryGetValue(assignment.AssignmentId, out var categoryId))
                continue;

            assignment.GradeCategoryId = !string.IsNullOrWhiteSpace(categoryId) && validCategoryIds.Contains(categoryId)
                ? categoryId
                : null;
        }

        await _context.SaveChangesAsync();

        var categories = await EnsureCategoriesAsync(courseId);
        var items = await LoadGradeItemsAsync(courseId, categories);
        return Ok(new GradebookPolicyDto
        {
            CourseId = courseId,
            CourseTitle = await _context.Courses.Where(c => c.CourseId == courseId).Select(c => c.Title).FirstOrDefaultAsync(),
            Categories = categories.Select(ToCategoryDto).ToList(),
            Items = items
        });
    }

    [HttpGet("course/{courseId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> GetCourseGradebook(string courseId)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var gradebook = await BuildCourseGradebook(courseId);
        if (gradebook == null)
            return NotFound(new { Message = "COURSE_NOT_FOUND" });

        return Ok(gradebook);
    }

    [HttpGet("course/{courseId}/me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyGrades(string courseId)
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

        if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
            return Forbid();

        var gradebook = await BuildCourseGradebook(courseId, studentId);
        if (gradebook == null)
            return NotFound(new { Message = "COURSE_NOT_FOUND" });

        var student = gradebook.Students.FirstOrDefault(s => s.StudentId == studentId);
        if (student == null)
            return NotFound(new { Message = "GRADEBOOK_STUDENT_NOT_FOUND" });

        return Ok(new
        {
            gradebook.CourseId,
            gradebook.CourseTitle,
            gradebook.Categories,
            gradebook.Items,
            Grade = student
        });
    }

    [HttpGet("course/{courseId}/export")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> ExportCourseGradebook(string courseId)
    {
        if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
            return Forbid();

        var gradebook = await BuildCourseGradebook(courseId);
        if (gradebook == null)
            return NotFound(new { Message = "COURSE_NOT_FOUND" });

        var csv = BuildCsv(gradebook);
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
        var safeTitle = string.Join("-", (gradebook.CourseTitle ?? courseId).Split(Path.GetInvalidFileNameChars()));
        return File(bytes, "text/csv", $"gradebook-{safeTitle}.csv");
    }

    private async Task<CourseGradebookDto?> BuildCourseGradebook(string courseId, string? onlyStudentId = null)
    {
        var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course == null)
            return null;

        var categories = await EnsureCategoriesAsync(courseId);
        var items = await LoadGradeItemsAsync(courseId, categories);
        var students = await LoadPaidStudentsAsync(courseId, onlyStudentId);
        var quizIds = items.Where(i => i.ItemType == "Quiz").Select(i => i.ItemId).ToList();
        var assignmentIds = items.Where(i => i.ItemType == "Assignment").Select(i => i.ItemId).ToList();
        var studentIds = students.Select(s => s.StudentId).ToList();

        var attempts = quizIds.Count == 0 || studentIds.Count == 0
            ? new List<QuizAttempt>()
            : await _context.QuizAttempts
                .AsNoTracking()
                .Where(a => quizIds.Contains(a.QuizId)
                    && studentIds.Contains(a.StudentId)
                    && a.Status != QuizAttemptStatus.PendingReview)
                .ToListAsync();

        var scores = attempts
            .GroupBy(a => new { a.StudentId, a.QuizId })
            .ToDictionary(
                g => (g.Key.StudentId, g.Key.QuizId),
                g =>
                {
                    var attempt = g.OrderByDescending(a => a.ScorePercent).ThenByDescending(a => a.SubmittedAt).First();
                    return new GradeScore(attempt.Score, attempt.TotalPoints, attempt.ScorePercent, attempt.Status, attempt.SubmittedAt);
                });

        if (assignmentIds.Count > 0 && studentIds.Count > 0)
        {
            var submissions = await _context.AssignmentSubmissions
                .AsNoTracking()
                .Where(s => assignmentIds.Contains(s.AssignmentId)
                    && studentIds.Contains(s.StudentId)
                    && s.ScorePercent.HasValue)
                .ToListAsync();

            foreach (var submission in submissions)
            {
                scores[(submission.StudentId, submission.AssignmentId)] = new GradeScore(
                    submission.Score,
                    null,
                    submission.ScorePercent,
                    submission.Status,
                    submission.SubmittedAt);
            }
        }

        var categoryDtos = categories.Select(ToCategoryDto).ToList();
        var studentGrades = students
            .Select(student => BuildStudentGrade(student, categoryDtos, items, scores))
            .OrderBy(s => s.StudentName ?? s.StudentEmail ?? s.StudentId)
            .ToList();

        return new CourseGradebookDto
        {
            CourseId = course.CourseId,
            CourseTitle = course.Title,
            Categories = categoryDtos,
            Items = items,
            Students = studentGrades,
            CurrentAveragePercent = AverageNullable(studentGrades.Select(s => s.CurrentPercent)),
            FinalAveragePercent = RoundGrade(studentGrades.Count == 0 ? 0 : studentGrades.Average(s => s.FinalPercent))
        };
    }

    private static StudentGradeSummaryDto BuildStudentGrade(
        Student student,
        List<GradeCategoryDto> categories,
        List<GradeItemDto> items,
        Dictionary<(string StudentId, string ItemId), GradeScore> scores)
    {
        var itemScores = items.Select(item =>
        {
            scores.TryGetValue((student.StudentId, item.ItemId), out var score);
            return new GradeItemScoreDto
            {
                ItemId = item.ItemId,
                Title = item.Title,
                ItemType = item.ItemType,
                CategoryId = item.CategoryId ?? string.Empty,
                PointsPossible = item.PointsPossible,
                Score = score?.Score,
                TotalPoints = score?.TotalPoints,
                ScorePercent = score?.ScorePercent,
                Status = score == null ? "Missing" : score.Status,
                SubmittedAt = score?.SubmittedAt
            };
        }).ToList();

        var categoryGrades = new List<CategoryGradeDto>();
        foreach (var category in categories.OrderBy(c => c.Order))
        {
            var categoryItems = itemScores.Where(i => i.CategoryId == category.CategoryId).ToList();
            var gradedItems = categoryItems.Where(i => i.ScorePercent.HasValue).ToList();
            var current = AverageNullable(gradedItems.Select(i => i.ScorePercent));
            var final = categoryItems.Count == 0
                ? 0
                : RoundGrade(categoryItems.Average(i => i.ScorePercent ?? 0));

            categoryGrades.Add(new CategoryGradeDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                WeightPercent = category.WeightPercent,
                CurrentPercent = current,
                FinalPercent = final,
                WeightedPoints = RoundGrade(final * category.WeightPercent / 100m),
                GradedItems = gradedItems.Count,
                TotalItems = categoryItems.Count
            });
        }

        var activeCategories = categoryGrades.Where(c => c.TotalItems > 0 && c.WeightPercent > 0).ToList();
        var gradedCategories = categoryGrades.Where(c => c.GradedItems > 0 && c.WeightPercent > 0).ToList();
        var finalPercent = NormalizeWeightedGrade(activeCategories, currentOnly: false);
        decimal? currentPercent = gradedCategories.Count == 0
            ? null
            : NormalizeWeightedGrade(gradedCategories, currentOnly: true);

        return new StudentGradeSummaryDto
        {
            StudentId = student.StudentId,
            StudentName = student.Name,
            StudentEmail = student.Email,
            CurrentPercent = currentPercent,
            FinalPercent = finalPercent,
            LetterGrade = ToLetterGrade(finalPercent),
            Categories = categoryGrades,
            Items = itemScores
        };
    }

    private async Task<List<GradeCategory>> EnsureCategoriesAsync(string courseId)
    {
        var categories = await _context.GradeCategories
            .Where(c => c.CourseId == courseId)
            .OrderBy(c => c.Order)
            .ToListAsync();

        if (categories.Count > 0)
            return categories;

        var now = DateTime.UtcNow;
        var category = new GradeCategory
        {
            CategoryId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            Name = DefaultCategoryName,
            WeightPercent = 100m,
            Order = 1,
            IsDefault = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        _context.GradeCategories.Add(category);
        await _context.SaveChangesAsync();
        return [category];
    }

    private async Task<List<GradeItemDto>> LoadGradeItemsAsync(string courseId, List<GradeCategory> categories)
    {
        var defaultCategoryId = categories.OrderBy(c => c.Order).First().CategoryId;
        var validCategoryIds = categories.Select(c => c.CategoryId).ToHashSet(StringComparer.Ordinal);

        var quizzes = await _context.CourseQuizzes
            .AsNoTracking()
            .Include(q => q.Questions)
            .Where(q => q.CourseId == courseId && q.IsPublished)
            .OrderBy(q => q.ScopeType)
            .ThenBy(q => q.CreatedAt)
            .ToListAsync();

        var quizItems = quizzes.Select(quiz => new GradeItemDto
        {
            ItemId = quiz.QuizId,
            Title = quiz.Title,
            ItemType = "Quiz",
            CategoryId = quiz.GradeCategoryId != null && validCategoryIds.Contains(quiz.GradeCategoryId)
                ? quiz.GradeCategoryId
                : defaultCategoryId,
            PointsPossible = quiz.Questions.Sum(q => q.Points) == 0 ? 100 : quiz.Questions.Sum(q => q.Points),
            IsPublished = quiz.IsPublished
        }).ToList();

        var assignments = await _context.CourseAssignments
            .AsNoTracking()
            .Where(a => a.CourseId == courseId && a.Status == AssignmentStatus.Published)
            .OrderBy(a => a.DueAt ?? DateTime.MaxValue)
            .ThenBy(a => a.Title)
            .ToListAsync();

        var assignmentItems = assignments.Select(assignment => new GradeItemDto
        {
            ItemId = assignment.AssignmentId,
            Title = assignment.Title,
            ItemType = "Assignment",
            CategoryId = assignment.GradeCategoryId != null && validCategoryIds.Contains(assignment.GradeCategoryId)
                ? assignment.GradeCategoryId
                : defaultCategoryId,
            PointsPossible = assignment.PointsPossible,
            IsPublished = assignment.Status == AssignmentStatus.Published
        });

        return quizItems.Concat(assignmentItems).ToList();
    }

    private async Task<List<Student>> LoadPaidStudentsAsync(string courseId, string? onlyStudentId)
    {
        var registrationStudents = await _context.CourseRegistrations
            .AsNoTracking()
            .Include(r => r.Student)
            .Where(r => r.CourseId == courseId
                && (r.PaymentStatus == PaymentStatus.Paid || r.PaymentStatus == PaymentStatus.Completed)
                && r.Student != null
                && (onlyStudentId == null || r.StudentId == onlyStudentId))
            .Select(r => r.Student!)
            .ToListAsync();

        var paymentStudents = await _context.Payments
            .AsNoTracking()
            .Include(p => p.Student)
            .Where(p => p.CourseId == courseId
                && (p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.Completed)
                && p.Student != null
                && (onlyStudentId == null || p.StudentId == onlyStudentId))
            .Select(p => p.Student!)
            .ToListAsync();

        return registrationStudents
            .Concat(paymentStudents)
            .GroupBy(s => s.StudentId)
            .Select(g => g.First())
            .OrderBy(s => s.Name ?? s.Email ?? s.StudentId)
            .ToList();
    }

    private static string? ValidatePolicy(UpdateGradebookPolicyDto dto)
    {
        if (dto.Categories.Count == 0)
            return "GRADEBOOK_CATEGORY_REQUIRED";

        if (dto.Categories.Any(c => string.IsNullOrWhiteSpace(c.Name)))
            return "GRADEBOOK_CATEGORY_NAME_REQUIRED";

        if (dto.Categories.Any(c => c.WeightPercent < 0 || c.WeightPercent > 100))
            return "GRADEBOOK_CATEGORY_WEIGHT_INVALID";

        var totalWeight = dto.Categories.Sum(c => c.WeightPercent);
        if (totalWeight < 99.99m || totalWeight > 100.01m)
            return "GRADEBOOK_CATEGORY_WEIGHTS_MUST_TOTAL_100";

        var duplicateName = dto.Categories
            .GroupBy(c => c.Name.Trim(), StringComparer.OrdinalIgnoreCase)
            .Any(g => g.Count() > 1);
        if (duplicateName)
            return "GRADEBOOK_CATEGORY_NAME_DUPLICATE";

        return null;
    }

    private static GradeCategoryDto ToCategoryDto(GradeCategory category) => new()
    {
        CategoryId = category.CategoryId,
        Name = category.Name,
        WeightPercent = category.WeightPercent,
        Order = category.Order,
        IsDefault = category.IsDefault
    };

    private static decimal? AverageNullable(IEnumerable<decimal?> values)
    {
        var materialized = values.Where(v => v.HasValue).Select(v => v!.Value).ToList();
        return materialized.Count == 0 ? null : RoundGrade(materialized.Average());
    }

    private static decimal NormalizeWeightedGrade(List<CategoryGradeDto> categories, bool currentOnly)
    {
        var totalWeight = categories.Sum(c => c.WeightPercent);
        if (totalWeight <= 0)
            return 0;

        var weighted = categories.Sum(c => (currentOnly ? c.CurrentPercent ?? 0 : c.FinalPercent) * c.WeightPercent);
        return RoundGrade(weighted / totalWeight);
    }

    private static decimal RoundGrade(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private static string ToLetterGrade(decimal percent)
    {
        return percent switch
        {
            >= 90m => "A",
            >= 80m => "B",
            >= 70m => "C",
            >= 60m => "D",
            _ => "F"
        };
    }

    private static string BuildCsv(CourseGradebookDto gradebook)
    {
        var builder = new StringBuilder();
        var headers = new List<string> { "Student Name", "Student Email", "Current %", "Final %", "Letter" };
        headers.AddRange(gradebook.Items.Select(i => i.Title));
        builder.AppendLine(string.Join(",", headers.Select(Csv)));

        foreach (var student in gradebook.Students)
        {
            var row = new List<string>
            {
                student.StudentName ?? student.StudentId,
                student.StudentEmail ?? string.Empty,
                student.CurrentPercent?.ToString("0.##") ?? string.Empty,
                student.FinalPercent.ToString("0.##"),
                student.LetterGrade
            };
            row.AddRange(gradebook.Items.Select(item =>
            {
                var score = student.Items.FirstOrDefault(i => i.ItemId == item.ItemId);
                return score?.ScorePercent?.ToString("0.##") ?? string.Empty;
            }));
            builder.AppendLine(string.Join(",", row.Select(Csv)));
        }

        return builder.ToString();
    }

    private static string Csv(string value)
    {
        value = value.Replace("\"", "\"\"");
        return $"\"{value}\"";
    }

    private string CurrentStudentId()
    {
        return User.FindFirstValue("studentId") ?? string.Empty;
    }

    private sealed record GradeScore(
        decimal? Score,
        decimal? TotalPoints,
        decimal? ScorePercent,
        string Status,
        DateTime? SubmittedAt);
}

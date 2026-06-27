using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Quiz;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly GymMarketContext _context;
        private readonly ICourseAccessService _courseAccessService;

        public QuizController(GymMarketContext context, ICourseAccessService courseAccessService)
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

            var quiz = await LoadQuiz(courseId);
            if (quiz == null)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            return Ok(ToTrainerDto(quiz));
        }

        [HttpPut("course/{courseId}")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> Upsert(string courseId, UpsertCourseQuizDto dto)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
                return Forbid();

            var validationError = ValidateQuiz(dto);
            if (validationError != null)
                return BadRequest(new { Message = validationError });

            var quiz = await _context.CourseQuizzes
                .Include(q => q.Questions).ThenInclude(q => q.Options)
                .Include(q => q.Attempts)
                .FirstOrDefaultAsync(q => q.CourseId == courseId);

            var now = DateTime.UtcNow;
            if (quiz == null)
            {
                quiz = new CourseQuiz
                {
                    QuizId = Guid.NewGuid().ToString(),
                    CourseId = courseId,
                    CreatedAt = now
                };
                _context.CourseQuizzes.Add(quiz);
            }
            else if (quiz.Attempts.Count > 0)
            {
                return Conflict(new { Message = "QUIZ_HAS_ATTEMPTS" });
            }
            else
            {
                _context.QuizOptions.RemoveRange(quiz.Questions.SelectMany(q => q.Options));
                _context.QuizQuestions.RemoveRange(quiz.Questions);
            }

            quiz.Title = dto.Title.Trim();
            quiz.PassingScorePercent = dto.PassingScorePercent;
            quiz.IsPublished = dto.IsPublished;
            quiz.UpdatedAt = now;
            quiz.Questions = dto.Questions
                .OrderBy(q => q.Order)
                .Select(question => new QuizQuestion
                {
                    QuestionId = Guid.NewGuid().ToString(),
                    QuizId = quiz.QuizId,
                    Prompt = question.Prompt.Trim(),
                    Order = question.Order,
                    Points = question.Points,
                    Options = question.Options.Select(option => new QuizOption
                    {
                        OptionId = Guid.NewGuid().ToString(),
                        Text = option.Text.Trim(),
                        IsCorrect = option.IsCorrect
                    }).ToList()
                })
                .ToList();

            await _context.SaveChangesAsync();
            return Ok(ToTrainerDto(quiz));
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

            var quiz = await LoadQuiz(courseId);
            if (quiz == null || !quiz.IsPublished)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            var attempts = await _context.QuizAttempts
                .AsNoTracking()
                .Where(a => a.QuizId == quiz.QuizId && a.StudentId == studentId)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();

            return Ok(ToStudentDto(quiz, attempts.FirstOrDefault(), attempts.OrderByDescending(a => a.ScorePercent).FirstOrDefault()));
        }

        [HttpPost("course/{courseId}/submit")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Submit(string courseId, SubmitQuizAttemptDto dto)
        {
            var studentId = CurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

            if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
                return Forbid();

            var quiz = await LoadQuiz(courseId);
            if (quiz == null || !quiz.IsPublished)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            var answersByQuestion = dto.Answers
                .GroupBy(a => a.QuestionId)
                .ToDictionary(g => g.Key, g => g.Last().SelectedOptionId);

            var totalPoints = quiz.Questions.Sum(q => q.Points);
            var score = 0;
            var attempt = new QuizAttempt
            {
                AttemptId = Guid.NewGuid().ToString(),
                QuizId = quiz.QuizId,
                StudentId = studentId,
                TotalPoints = totalPoints,
                SubmittedAt = DateTime.UtcNow
            };

            foreach (var question in quiz.Questions)
            {
                answersByQuestion.TryGetValue(question.QuestionId, out var selectedOptionId);
                var selected = question.Options.FirstOrDefault(o => o.OptionId == selectedOptionId);
                var isCorrect = selected?.IsCorrect == true;
                var points = isCorrect ? question.Points : 0;
                score += points;

                attempt.Answers.Add(new QuizAttemptAnswer
                {
                    AttemptAnswerId = Guid.NewGuid().ToString(),
                    AttemptId = attempt.AttemptId,
                    QuestionId = question.QuestionId,
                    SelectedOptionId = selected?.OptionId,
                    IsCorrect = isCorrect,
                    PointsAwarded = points
                });
            }

            attempt.Score = score;
            attempt.ScorePercent = totalPoints == 0 ? 0 : Math.Round((decimal)score / totalPoints * 100, 2);
            attempt.Passed = attempt.ScorePercent >= quiz.PassingScorePercent;

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            return Ok(ToAttemptDto(attempt));
        }

        [HttpGet("course/{courseId}/gradebook")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> GetGradebook(string courseId)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
                return Forbid();

            var quiz = await _context.CourseQuizzes
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.CourseId == courseId);
            if (quiz == null)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            var attempts = await _context.QuizAttempts
                .AsNoTracking()
                .Include(a => a.Student)
                .Where(a => a.QuizId == quiz.QuizId)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();

            return Ok(attempts.Select(ToAttemptDto));
        }

        private async Task<CourseQuiz?> LoadQuiz(string courseId)
        {
            return await _context.CourseQuizzes
                .Include(q => q.Questions.OrderBy(question => question.Order))
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.CourseId == courseId);
        }

        private static string? ValidateQuiz(UpsertCourseQuizDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return "QUIZ_TITLE_REQUIRED";

            if (dto.IsPublished && dto.Questions.Count == 0)
                return "PUBLISHED_QUIZ_REQUIRES_QUESTIONS";

            foreach (var question in dto.Questions)
            {
                if (string.IsNullOrWhiteSpace(question.Prompt))
                    return "QUESTION_PROMPT_REQUIRED";
                if (question.Options.Count < 2)
                    return "QUESTION_REQUIRES_TWO_OPTIONS";
                if (question.Options.Count(o => o.IsCorrect) != 1)
                    return "QUESTION_REQUIRES_ONE_CORRECT_OPTION";
                if (question.Options.Any(o => string.IsNullOrWhiteSpace(o.Text)))
                    return "OPTION_TEXT_REQUIRED";
            }

            return null;
        }

        private static TrainerCourseQuizDto ToTrainerDto(CourseQuiz quiz)
        {
            return new TrainerCourseQuizDto
            {
                QuizId = quiz.QuizId,
                CourseId = quiz.CourseId,
                Title = quiz.Title,
                PassingScorePercent = quiz.PassingScorePercent,
                IsPublished = quiz.IsPublished,
                Questions = quiz.Questions
                    .OrderBy(q => q.Order)
                    .Select(q => new TrainerQuizQuestionDto
                    {
                        QuestionId = q.QuestionId,
                        Prompt = q.Prompt,
                        Order = q.Order,
                        Points = q.Points,
                        Options = q.Options.Select(o => new TrainerQuizOptionDto
                        {
                            OptionId = o.OptionId,
                            Text = o.Text,
                            IsCorrect = o.IsCorrect
                        }).ToList()
                    }).ToList()
            };
        }

        private static CourseQuizDto ToStudentDto(CourseQuiz quiz, QuizAttempt? latestAttempt, QuizAttempt? bestAttempt)
        {
            return new CourseQuizDto
            {
                QuizId = quiz.QuizId,
                CourseId = quiz.CourseId,
                Title = quiz.Title,
                PassingScorePercent = quiz.PassingScorePercent,
                IsPublished = quiz.IsPublished,
                LatestAttempt = latestAttempt == null ? null : ToAttemptDto(latestAttempt),
                BestAttempt = bestAttempt == null ? null : ToAttemptDto(bestAttempt),
                Questions = quiz.Questions
                    .OrderBy(q => q.Order)
                    .Select(q => new QuizQuestionDto
                    {
                        QuestionId = q.QuestionId,
                        Prompt = q.Prompt,
                        Order = q.Order,
                        Points = q.Points,
                        Options = q.Options.Select(o => new QuizOptionDto
                        {
                            OptionId = o.OptionId,
                            Text = o.Text
                        }).ToList()
                    }).ToList()
            };
        }

        private static QuizAttemptSummaryDto ToAttemptDto(QuizAttempt attempt)
        {
            return new QuizAttemptSummaryDto
            {
                AttemptId = attempt.AttemptId,
                QuizId = attempt.QuizId,
                StudentId = attempt.StudentId,
                StudentName = attempt.Student?.Name,
                StudentEmail = attempt.Student?.Email,
                Score = attempt.Score,
                TotalPoints = attempt.TotalPoints,
                ScorePercent = attempt.ScorePercent,
                Passed = attempt.Passed,
                SubmittedAt = attempt.SubmittedAt
            };
        }

        private string CurrentStudentId()
        {
            return User.FindFirstValue("studentId") ?? string.Empty;
        }
    }
}

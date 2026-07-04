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

            var quiz = await LoadPrimaryQuiz(courseId);
            if (quiz == null)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            return Ok(ToTrainerDto(quiz));
        }

        [HttpGet("course/{courseId}/manage/all")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> GetAllForManagement(string courseId)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
                return Forbid();

            var quizzes = await _context.CourseQuizzes
                .AsNoTracking()
                .Include(q => q.Module)
                .Include(q => q.Lecture)
                .Include(q => q.Questions.OrderBy(question => question.Order))
                    .ThenInclude(q => q.Options)
                .Where(q => q.CourseId == courseId)
                .OrderBy(q => q.ScopeType)
                .ThenBy(q => q.CreatedAt)
                .ToListAsync();

            return Ok(quizzes.Select(ToTrainerDto));
        }

        [HttpGet("{quizId}/manage")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> GetAssessmentForManagement(string quizId)
        {
            var quiz = await LoadQuizById(quizId);
            if (quiz == null)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            if (!await _courseAccessService.CanManageCourseAsync(User, quiz.CourseId))
                return Forbid();

            return Ok(ToTrainerDto(quiz));
        }

        [HttpPost("course/{courseId}")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> Create(string courseId, UpsertCourseQuizDto dto)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
                return Forbid();

            var validationError = await ValidateQuiz(courseId, null, dto);
            if (validationError != null)
                return BadRequest(new { Message = validationError });

            var now = DateTime.UtcNow;
            var quiz = new CourseQuiz
            {
                QuizId = Guid.NewGuid().ToString(),
                CourseId = courseId,
                CreatedAt = now
            };

            ApplyQuiz(quiz, dto, now);
            _context.CourseQuizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return Ok(ToTrainerDto(quiz));
        }

        [HttpPut("course/{courseId}")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> Upsert(string courseId, UpsertCourseQuizDto dto)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
                return Forbid();

            var quiz = await _context.CourseQuizzes
                .Include(q => q.Questions).ThenInclude(q => q.Options)
                .Include(q => q.Attempts)
                .OrderBy(q => q.CreatedAt)
                .FirstOrDefaultAsync(q => q.CourseId == courseId && q.ScopeType == AssessmentScopeType.Course);

            return await SaveQuiz(courseId, quiz, dto);
        }

        [HttpPut("{quizId}")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> Update(string quizId, UpsertCourseQuizDto dto)
        {
            var quiz = await _context.CourseQuizzes
                .Include(q => q.Questions).ThenInclude(q => q.Options)
                .Include(q => q.Attempts)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            if (!await _courseAccessService.CanManageCourseAsync(User, quiz.CourseId))
                return Forbid();

            return await SaveQuiz(quiz.CourseId, quiz, dto);
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

            var quiz = await LoadPrimaryQuiz(courseId);
            if (quiz == null || !quiz.IsPublished)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            var accessError = await ValidateStudentAssessmentAccess(quiz, studentId);
            if (accessError != null)
                return Conflict(new { Message = accessError });

            return Ok(await ToStudentDto(quiz, studentId));
        }

        [HttpGet("course/{courseId}/all")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetAllForStudent(string courseId)
        {
            var studentId = CurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

            if (!await _courseAccessService.CanAccessCourseAsync(User, courseId))
                return Forbid();

            var quizzes = await _context.CourseQuizzes
                .AsNoTracking()
                .Include(q => q.Module)
                .Include(q => q.Lecture)
                .Include(q => q.Questions.OrderBy(question => question.Order))
                    .ThenInclude(q => q.Options)
                .Where(q => q.CourseId == courseId && q.IsPublished)
                .OrderBy(q => q.ScopeType)
                .ThenBy(q => q.CreatedAt)
                .ToListAsync();

            var available = new List<CourseQuizDto>();
            foreach (var quiz in quizzes)
            {
                if (await ValidateStudentAssessmentAccess(quiz, studentId) == null)
                    available.Add(await ToStudentDto(quiz, studentId));
            }

            return Ok(available);
        }

        [HttpPost("course/{courseId}/submit")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitForCourse(string courseId, SubmitQuizAttemptDto dto)
        {
            var quiz = await LoadPrimaryQuiz(courseId);
            if (quiz == null)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            return await SubmitAssessment(quiz, dto);
        }

        [HttpPost("{quizId}/submit")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Submit(string quizId, SubmitQuizAttemptDto dto)
        {
            var quiz = await LoadQuizById(quizId);
            if (quiz == null)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            return await SubmitAssessment(quiz, dto);
        }

        [HttpGet("course/{courseId}/gradebook")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> GetGradebook(string courseId)
        {
            if (!await _courseAccessService.CanManageCourseAsync(User, courseId))
                return Forbid();

            var quiz = await _context.CourseQuizzes
                .AsNoTracking()
                .OrderBy(q => q.CreatedAt)
                .FirstOrDefaultAsync(q => q.CourseId == courseId && q.ScopeType == AssessmentScopeType.Course);
            if (quiz == null)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            return Ok(await LoadGradebook(quiz.QuizId));
        }

        [HttpGet("{quizId}/gradebook")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> GetAssessmentGradebook(string quizId)
        {
            var quiz = await _context.CourseQuizzes.AsNoTracking().FirstOrDefaultAsync(q => q.QuizId == quizId);
            if (quiz == null)
                return NotFound(new { Message = "QUIZ_NOT_FOUND" });

            if (!await _courseAccessService.CanManageCourseAsync(User, quiz.CourseId))
                return Forbid();

            return Ok(await LoadGradebook(quizId));
        }

        [HttpPut("attempts/{attemptId}/grade")]
        [Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> GradeAttempt(string attemptId, GradeQuizAttemptDto dto)
        {
            var attempt = await _context.QuizAttempts
                .Include(a => a.Quiz)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.AttemptId == attemptId);
            if (attempt?.Quiz == null)
                return NotFound(new { Message = "QUIZ_ATTEMPT_NOT_FOUND" });

            if (!await _courseAccessService.CanManageCourseAsync(User, attempt.Quiz.CourseId))
                return Forbid();

            attempt.Score = Math.Clamp(dto.Score, 0, attempt.TotalPoints);
            attempt.ScorePercent = attempt.TotalPoints == 0
                ? 0
                : Math.Round((decimal)attempt.Score / attempt.TotalPoints * 100, 2);
            attempt.Passed = attempt.ScorePercent >= attempt.Quiz.PassingScorePercent;
            attempt.Status = QuizAttemptStatus.Graded;
            attempt.RequiresManualGrading = false;
            attempt.GradedAt = DateTime.UtcNow;
            attempt.Feedback = string.IsNullOrWhiteSpace(dto.Feedback) ? null : dto.Feedback.Trim();

            await _context.SaveChangesAsync();
            return Ok(ToAttemptDto(attempt));
        }

        private async Task<IActionResult> SaveQuiz(string courseId, CourseQuiz? quiz, UpsertCourseQuizDto dto)
        {
            var validationError = await ValidateQuiz(courseId, quiz?.QuizId, dto);
            if (validationError != null)
                return BadRequest(new { Message = validationError });

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

            ApplyQuiz(quiz, dto, now);
            await _context.SaveChangesAsync();

            return Ok(ToTrainerDto(quiz));
        }

        private void ApplyQuiz(CourseQuiz quiz, UpsertCourseQuizDto dto, DateTime now)
        {
            var scopeType = AssessmentScopeType.Normalize(dto.ScopeType);

            quiz.Title = dto.Title.Trim();
            quiz.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            quiz.ScopeType = scopeType;
            quiz.ModuleId = scopeType == AssessmentScopeType.Module ? dto.ModuleId : null;
            quiz.LectureId = scopeType == AssessmentScopeType.Lesson ? dto.LectureId : null;
            quiz.PassingScorePercent = dto.PassingScorePercent;
            quiz.TimeLimitMinutes = dto.TimeLimitMinutes;
            quiz.MaxAttempts = dto.MaxAttempts;
            quiz.ShuffleQuestions = dto.ShuffleQuestions;
            quiz.ShuffleOptions = dto.ShuffleOptions;
            quiz.ShowCorrectAnswers = dto.ShowCorrectAnswers;
            quiz.RequireHonorCode = dto.RequireHonorCode;
            quiz.TrackProctoringSignals = dto.TrackProctoringSignals;
            quiz.AvailableFrom = dto.AvailableFrom;
            quiz.AvailableUntil = dto.AvailableUntil;
            quiz.IsPublished = dto.IsPublished;
            quiz.UpdatedAt = now;
            quiz.Questions = dto.Questions
                .OrderBy(q => q.Order)
                .Select(question =>
                {
                    var questionType = QuizQuestionType.Normalize(question.QuestionType);
                    return new QuizQuestion
                    {
                        QuestionId = Guid.NewGuid().ToString(),
                        QuizId = quiz.QuizId,
                        Prompt = question.Prompt.Trim(),
                        QuestionType = questionType,
                        Order = question.Order,
                        Points = question.Points,
                        Explanation = string.IsNullOrWhiteSpace(question.Explanation) ? null : question.Explanation.Trim(),
                        QuestionBank = string.IsNullOrWhiteSpace(question.QuestionBank) ? null : question.QuestionBank.Trim(),
                        RequiresManualGrading = questionType == QuizQuestionType.OpenText,
                        Options = questionType == QuizQuestionType.OpenText
                            ? []
                            : question.Options.Select(option => new QuizOption
                            {
                                OptionId = Guid.NewGuid().ToString(),
                                Text = option.Text.Trim(),
                                IsCorrect = option.IsCorrect
                            }).ToList()
                    };
                })
                .ToList();
        }

        private async Task<CourseQuiz?> LoadPrimaryQuiz(string courseId)
        {
            return await _context.CourseQuizzes
                .Include(q => q.Module)
                .Include(q => q.Lecture)
                .Include(q => q.Questions.OrderBy(question => question.Order))
                    .ThenInclude(q => q.Options)
                .OrderBy(q => q.CreatedAt)
                .FirstOrDefaultAsync(q => q.CourseId == courseId && q.ScopeType == AssessmentScopeType.Course);
        }

        private async Task<CourseQuiz?> LoadQuizById(string quizId)
        {
            return await _context.CourseQuizzes
                .Include(q => q.Module)
                .Include(q => q.Lecture)
                .Include(q => q.Questions.OrderBy(question => question.Order))
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);
        }

        private async Task<List<QuizAttemptSummaryDto>> LoadGradebook(string quizId)
        {
            var attempts = await _context.QuizAttempts
                .AsNoTracking()
                .Include(a => a.Student)
                .Where(a => a.QuizId == quizId)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();

            return attempts.Select(ToAttemptDto).ToList();
        }

        private async Task<string?> ValidateQuiz(string courseId, string? quizId, UpsertCourseQuizDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return "QUIZ_TITLE_REQUIRED";

            var scopeType = AssessmentScopeType.Normalize(dto.ScopeType);
            if (scopeType == AssessmentScopeType.Module)
            {
                if (string.IsNullOrWhiteSpace(dto.ModuleId))
                    return "MODULE_ASSESSMENT_REQUIRES_MODULE";

                var moduleExists = await _context.CourseModules.AnyAsync(m => m.ModuleId == dto.ModuleId && m.CourseId == courseId);
                if (!moduleExists)
                    return "MODULE_NOT_IN_COURSE";
            }

            if (scopeType == AssessmentScopeType.Lesson)
            {
                if (string.IsNullOrWhiteSpace(dto.LectureId))
                    return "LESSON_ASSESSMENT_REQUIRES_LESSON";

                var lectureExists = await _context.Lectures.AnyAsync(l => l.LectureId == dto.LectureId && l.CourseId == courseId);
                if (!lectureExists)
                    return "LESSON_NOT_IN_COURSE";
            }

            if (dto.AvailableFrom.HasValue && dto.AvailableUntil.HasValue && dto.AvailableUntil <= dto.AvailableFrom)
                return "ASSESSMENT_AVAILABLE_UNTIL_MUST_BE_AFTER_FROM";

            if (dto.MaxAttempts.HasValue && dto.MaxAttempts.Value <= 0)
                return "MAX_ATTEMPTS_INVALID";

            if (dto.IsPublished && dto.Questions.Count == 0)
                return "PUBLISHED_ASSESSMENT_REQUIRES_QUESTIONS";

            foreach (var question in dto.Questions)
            {
                var questionType = QuizQuestionType.Normalize(question.QuestionType);
                if (string.IsNullOrWhiteSpace(question.Prompt))
                    return "QUESTION_PROMPT_REQUIRED";

                if (questionType == QuizQuestionType.OpenText)
                    continue;

                if (question.Options.Count < 2)
                    return "QUESTION_REQUIRES_TWO_OPTIONS";
                if (questionType == QuizQuestionType.SingleChoice && question.Options.Count(o => o.IsCorrect) != 1)
                    return "QUESTION_REQUIRES_ONE_CORRECT_OPTION";
                if (questionType == QuizQuestionType.MultipleChoice && question.Options.Count(o => o.IsCorrect) == 0)
                    return "QUESTION_REQUIRES_CORRECT_OPTION";
                if (question.Options.Any(o => string.IsNullOrWhiteSpace(o.Text)))
                    return "OPTION_TEXT_REQUIRED";
            }

            return null;
        }

        private async Task<string?> ValidateStudentAssessmentAccess(CourseQuiz quiz, string studentId)
        {
            var now = DateTime.UtcNow;
            if (!quiz.IsPublished)
                return "QUIZ_NOT_PUBLISHED";
            if (quiz.AvailableFrom.HasValue && quiz.AvailableFrom.Value > now)
                return "ASSESSMENT_NOT_AVAILABLE_YET";
            if (quiz.AvailableUntil.HasValue && quiz.AvailableUntil.Value < now)
                return "ASSESSMENT_CLOSED";

            if (quiz.ScopeType == AssessmentScopeType.Lesson && !string.IsNullOrEmpty(quiz.LectureId))
            {
                var unlockState = await _courseAccessService.GetLectureUnlockStateAsync(User, quiz.LectureId);
                if (unlockState.IsLocked)
                    return unlockState.Reason ?? "LESSON_LOCKED";
            }

            var attemptCount = await _context.QuizAttempts.CountAsync(a => a.QuizId == quiz.QuizId && a.StudentId == studentId);
            if (quiz.MaxAttempts.HasValue && attemptCount >= quiz.MaxAttempts.Value)
                return "MAX_ATTEMPTS_REACHED";

            return null;
        }

        private async Task<IActionResult> SubmitAssessment(CourseQuiz quiz, SubmitQuizAttemptDto dto)
        {
            var studentId = CurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });

            if (!await _courseAccessService.CanAccessCourseAsync(User, quiz.CourseId))
                return Forbid();

            var accessError = await ValidateStudentAssessmentAccess(quiz, studentId);
            if (accessError != null)
                return Conflict(new { Message = accessError });

            if (quiz.RequireHonorCode && !dto.HonorCodeAccepted)
                return BadRequest(new { Message = "HONOR_CODE_REQUIRED" });

            var timingError = ValidateAttemptTiming(quiz, dto);
            if (timingError != null)
                return Conflict(new { Message = timingError });

            var answerValidationError = ValidateSubmittedAnswers(quiz, dto);
            if (answerValidationError != null)
                return BadRequest(new { Message = answerValidationError });

            var answersByQuestion = dto.Answers
                .GroupBy(a => a.QuestionId)
                .ToDictionary(g => g.Key, g => g.Last());

            var attemptCount = await _context.QuizAttempts.CountAsync(a => a.QuizId == quiz.QuizId && a.StudentId == studentId);
            var totalPoints = quiz.Questions.Sum(q => q.Points);
            var score = 0;
            var requiresManualGrading = quiz.Questions.Any(q => q.RequiresManualGrading);
            var proctoring = AnalyzeProctoring(dto.ProctoringSignals);
            var attempt = new QuizAttempt
            {
                AttemptId = Guid.NewGuid().ToString(),
                QuizId = quiz.QuizId,
                StudentId = studentId,
                AttemptNumber = attemptCount + 1,
                TotalPoints = totalPoints,
                StartedAt = dto.StartedAt?.ToUniversalTime(),
                SubmittedAt = DateTime.UtcNow,
                Status = requiresManualGrading ? QuizAttemptStatus.PendingReview : QuizAttemptStatus.Graded,
                RequiresManualGrading = requiresManualGrading,
                QuestionOrderSnapshot = dto.Answers.Count == 0 ? null : string.Join(",", dto.Answers.Select(a => a.QuestionId)),
                BrowserFingerprint = string.IsNullOrWhiteSpace(dto.BrowserFingerprint) ? null : dto.BrowserFingerprint.Trim(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                HonorCodeAccepted = dto.HonorCodeAccepted,
                FocusLostCount = proctoring.FocusLostCount,
                PasteEventCount = proctoring.PasteEventCount,
                FullscreenExitCount = proctoring.FullscreenExitCount,
                ProctoringEventCount = proctoring.EventCount,
                SuspiciousActivityScore = proctoring.Score,
                ProctoringReviewRequired = quiz.TrackProctoringSignals && proctoring.Score >= 5,
                ProctoringFlags = quiz.TrackProctoringSignals ? proctoring.Flags : null
            };

            foreach (var question in quiz.Questions)
            {
                answersByQuestion.TryGetValue(question.QuestionId, out var answer);
                var questionType = QuizQuestionType.Normalize(question.QuestionType);
                var selectedOptionIds = NormalizeSelectedOptionIds(answer);
                var selectedOptionId = selectedOptionIds.FirstOrDefault();
                var isCorrect = false;
                var points = 0;

                if (questionType == QuizQuestionType.SingleChoice)
                {
                    var selected = question.Options.FirstOrDefault(o => o.OptionId == selectedOptionId);
                    isCorrect = selected?.IsCorrect == true;
                    points = isCorrect ? question.Points : 0;
                }
                else if (questionType == QuizQuestionType.MultipleChoice)
                {
                    var correctIds = question.Options.Where(o => o.IsCorrect).Select(o => o.OptionId).Order().ToList();
                    var submittedIds = selectedOptionIds.Order().ToList();
                    isCorrect = correctIds.SequenceEqual(submittedIds);
                    points = isCorrect ? question.Points : 0;
                }

                score += points;
                attempt.Answers.Add(new QuizAttemptAnswer
                {
                    AttemptAnswerId = Guid.NewGuid().ToString(),
                    AttemptId = attempt.AttemptId,
                    QuestionId = question.QuestionId,
                    SelectedOptionId = questionType == QuizQuestionType.SingleChoice ? selectedOptionId : null,
                    SelectedOptionIds = selectedOptionIds.Count == 0 ? null : string.Join(",", selectedOptionIds),
                    TextAnswer = string.IsNullOrWhiteSpace(answer?.TextAnswer) ? null : answer.TextAnswer.Trim(),
                    IsCorrect = isCorrect,
                    PointsAwarded = points
                });
            }

            attempt.Score = score;
            attempt.ScorePercent = totalPoints == 0 ? 0 : Math.Round((decimal)score / totalPoints * 100, 2);
            attempt.Passed = !requiresManualGrading && attempt.ScorePercent >= quiz.PassingScorePercent;

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            return Ok(ToAttemptDto(attempt));
        }

        private static string? ValidateAttemptTiming(CourseQuiz quiz, SubmitQuizAttemptDto dto)
        {
            if (!quiz.TimeLimitMinutes.HasValue || !dto.StartedAt.HasValue)
                return null;

            var startedAt = dto.StartedAt.Value.ToUniversalTime();
            var latestSubmitAt = startedAt.AddMinutes(quiz.TimeLimitMinutes.Value + 2);
            return DateTime.UtcNow > latestSubmitAt ? "ASSESSMENT_TIME_LIMIT_EXCEEDED" : null;
        }

        private static string? ValidateSubmittedAnswers(CourseQuiz quiz, SubmitQuizAttemptDto dto)
        {
            var questionIds = quiz.Questions.Select(q => q.QuestionId).ToHashSet(StringComparer.Ordinal);
            var submittedIds = dto.Answers.Select(a => a.QuestionId).ToList();
            if (submittedIds.Any(id => !questionIds.Contains(id)))
                return "QUESTION_NOT_IN_QUIZ";

            if (submittedIds.Distinct(StringComparer.Ordinal).Count() != submittedIds.Count)
                return "DUPLICATE_QUESTION_ANSWER";

            foreach (var question in quiz.Questions)
            {
                var answer = dto.Answers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
                if (answer == null)
                    return "ANSWER_REQUIRED";

                var questionType = QuizQuestionType.Normalize(question.QuestionType);
                if (questionType == QuizQuestionType.OpenText)
                {
                    if (string.IsNullOrWhiteSpace(answer.TextAnswer))
                        return "TEXT_ANSWER_REQUIRED";
                    continue;
                }

                var optionIds = NormalizeSelectedOptionIds(answer);
                if (optionIds.Count == 0)
                    return "OPTION_ANSWER_REQUIRED";

                var validOptionIds = question.Options.Select(o => o.OptionId).ToHashSet(StringComparer.Ordinal);
                if (optionIds.Any(optionId => !validOptionIds.Contains(optionId)))
                    return "OPTION_NOT_IN_QUESTION";

                if (questionType == QuizQuestionType.SingleChoice && optionIds.Count != 1)
                    return "SINGLE_CHOICE_REQUIRES_ONE_OPTION";
            }

            return null;
        }

        private static List<string> NormalizeSelectedOptionIds(SubmitQuizAnswerDto? answer)
        {
            var ids = new List<string>();
            if (!string.IsNullOrWhiteSpace(answer?.SelectedOptionId))
                ids.Add(answer.SelectedOptionId);

            if (answer?.SelectedOptionIds != null)
                ids.AddRange(answer.SelectedOptionIds.Where(id => !string.IsNullOrWhiteSpace(id)));

            return ids.Distinct().ToList();
        }

        private static TrainerCourseQuizDto ToTrainerDto(CourseQuiz quiz)
        {
            return new TrainerCourseQuizDto
            {
                QuizId = quiz.QuizId,
                CourseId = quiz.CourseId,
                Title = quiz.Title,
                Description = quiz.Description,
                ScopeType = quiz.ScopeType,
                ModuleId = quiz.ModuleId,
                ModuleTitle = quiz.Module?.Title,
                LectureId = quiz.LectureId,
                LectureTitle = quiz.Lecture?.Title,
                PassingScorePercent = quiz.PassingScorePercent,
                TimeLimitMinutes = quiz.TimeLimitMinutes,
                MaxAttempts = quiz.MaxAttempts,
                ShuffleQuestions = quiz.ShuffleQuestions,
                ShuffleOptions = quiz.ShuffleOptions,
                ShowCorrectAnswers = quiz.ShowCorrectAnswers,
                RequireHonorCode = quiz.RequireHonorCode,
                TrackProctoringSignals = quiz.TrackProctoringSignals,
                AvailableFrom = quiz.AvailableFrom,
                AvailableUntil = quiz.AvailableUntil,
                IsPublished = quiz.IsPublished,
                Questions = quiz.Questions
                    .OrderBy(q => q.Order)
                    .Select(q => new TrainerQuizQuestionDto
                    {
                        QuestionId = q.QuestionId,
                        Prompt = q.Prompt,
                        QuestionType = q.QuestionType,
                        Order = q.Order,
                        Points = q.Points,
                        Explanation = q.Explanation,
                        QuestionBank = q.QuestionBank,
                        RequiresManualGrading = q.RequiresManualGrading,
                        Options = q.Options.Select(o => new TrainerQuizOptionDto
                        {
                            OptionId = o.OptionId,
                            Text = o.Text,
                            IsCorrect = o.IsCorrect
                        }).ToList()
                    }).ToList()
            };
        }

        private async Task<CourseQuizDto> ToStudentDto(CourseQuiz quiz, string studentId)
        {
            var attempts = await _context.QuizAttempts
                .AsNoTracking()
                .Where(a => a.QuizId == quiz.QuizId && a.StudentId == studentId)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();

            var questions = quiz.ShuffleQuestions
                ? quiz.Questions.OrderBy(_ => Guid.NewGuid()).ToList()
                : quiz.Questions.OrderBy(q => q.Order).ToList();

            return new CourseQuizDto
            {
                QuizId = quiz.QuizId,
                CourseId = quiz.CourseId,
                Title = quiz.Title,
                Description = quiz.Description,
                ScopeType = quiz.ScopeType,
                ModuleId = quiz.ModuleId,
                ModuleTitle = quiz.Module?.Title,
                LectureId = quiz.LectureId,
                LectureTitle = quiz.Lecture?.Title,
                PassingScorePercent = quiz.PassingScorePercent,
                TimeLimitMinutes = quiz.TimeLimitMinutes,
                MaxAttempts = quiz.MaxAttempts,
                AttemptsUsed = attempts.Count,
                AttemptsRemaining = quiz.MaxAttempts.HasValue ? Math.Max(quiz.MaxAttempts.Value - attempts.Count, 0) : null,
                ShuffleQuestions = quiz.ShuffleQuestions,
                ShuffleOptions = quiz.ShuffleOptions,
                ShowCorrectAnswers = quiz.ShowCorrectAnswers,
                RequireHonorCode = quiz.RequireHonorCode,
                TrackProctoringSignals = quiz.TrackProctoringSignals,
                AvailableFrom = quiz.AvailableFrom,
                AvailableUntil = quiz.AvailableUntil,
                IsPublished = quiz.IsPublished,
                LatestAttempt = attempts.FirstOrDefault() == null ? null : ToAttemptDto(attempts.First()),
                BestAttempt = attempts.OrderByDescending(a => a.ScorePercent).FirstOrDefault() == null
                    ? null
                    : ToAttemptDto(attempts.OrderByDescending(a => a.ScorePercent).First()),
                Questions = questions
                    .Select(q => new QuizQuestionDto
                    {
                        QuestionId = q.QuestionId,
                        Prompt = q.Prompt,
                        QuestionType = q.QuestionType,
                        Order = q.Order,
                        Points = q.Points,
                        Explanation = quiz.ShowCorrectAnswers ? q.Explanation : null,
                        QuestionBank = q.QuestionBank,
                        Options = ToStudentOptions(q, quiz.ShuffleOptions)
                    }).ToList()
            };
        }

        private static List<QuizOptionDto> ToStudentOptions(QuizQuestion question, bool shuffleOptions)
        {
            var options = shuffleOptions
                ? question.Options.OrderBy(_ => Guid.NewGuid()).ToList()
                : question.Options.ToList();

            return options.Select(o => new QuizOptionDto
            {
                OptionId = o.OptionId,
                Text = o.Text
            }).ToList();
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
                AttemptNumber = attempt.AttemptNumber,
                Score = attempt.Score,
                TotalPoints = attempt.TotalPoints,
                ScorePercent = attempt.ScorePercent,
                Passed = attempt.Passed,
                Status = attempt.Status,
                RequiresManualGrading = attempt.RequiresManualGrading,
                StartedAt = attempt.StartedAt,
                HonorCodeAccepted = attempt.HonorCodeAccepted,
                ProctoringReviewRequired = attempt.ProctoringReviewRequired,
                FocusLostCount = attempt.FocusLostCount,
                PasteEventCount = attempt.PasteEventCount,
                FullscreenExitCount = attempt.FullscreenExitCount,
                ProctoringEventCount = attempt.ProctoringEventCount,
                SuspiciousActivityScore = attempt.SuspiciousActivityScore,
                ProctoringFlags = attempt.ProctoringFlags,
                Feedback = attempt.Feedback,
                SubmittedAt = attempt.SubmittedAt
            };
        }

        private static ProctoringStats AnalyzeProctoring(List<QuizProctoringSignalDto> signals)
        {
            var focusLost = SignalCount(signals, "focus_lost", "blur", "tab_hidden");
            var paste = SignalCount(signals, "paste");
            var fullscreenExit = SignalCount(signals, "fullscreen_exit");
            var eventCount = signals.Sum(signal => Math.Max(signal.Count, 1));
            var score = focusLost + (paste * 2) + fullscreenExit;
            var flags = new List<string>();
            if (focusLost > 0) flags.Add($"{focusLost} focus changes");
            if (paste > 0) flags.Add($"{paste} paste events");
            if (fullscreenExit > 0) flags.Add($"{fullscreenExit} fullscreen exits");

            return new ProctoringStats(
                focusLost,
                paste,
                fullscreenExit,
                eventCount,
                score,
                flags.Count == 0 ? null : string.Join("; ", flags));
        }

        private static int SignalCount(List<QuizProctoringSignalDto> signals, params string[] types)
        {
            var allowed = types.ToHashSet(StringComparer.OrdinalIgnoreCase);
            return signals
                .Where(signal => allowed.Contains(signal.Type.Trim()))
                .Sum(signal => Math.Max(signal.Count, 1));
        }

        private sealed record ProctoringStats(
            int FocusLostCount,
            int PasteEventCount,
            int FullscreenExitCount,
            int EventCount,
            int Score,
            string? Flags);

        private string CurrentStudentId()
        {
            return User.FindFirstValue("studentId") ?? string.Empty;
        }
    }
}

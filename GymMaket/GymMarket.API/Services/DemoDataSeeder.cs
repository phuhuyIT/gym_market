using GymMarket.API.Data;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Services;

public static class DemoDataSeedIds
{
    public const string AdminEmail = "demo.admin@gymmarket.local";
    public const string TrainerEmail = "demo.trainer@gymmarket.local";
    public const string StudentEmail = "demo.student@gymmarket.local";
    public const string Password = "Password123";

    public const string TrainerId = "DEMO-TRAINER-001";
    public const string StudentId = "DEMO-STUDENT-001";
    public const string CourseId = "DEMO-LMS-COURSE";
}

public sealed class DemoDataSeeder : IDemoDataSeeder
{
    private readonly GymMarketContext _context;
    private readonly UserManager<AppUser> _userManager;

    public DemoDataSeeder(GymMarketContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<DemoDataSeedResult> EnsureSeededAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Courses.AnyAsync(c => c.CourseId == DemoDataSeedIds.CourseId, cancellationToken))
        {
            return Result(created: false);
        }

        var now = DateTime.UtcNow;
        var adminUser = await EnsureUserAsync(DemoDataSeedIds.AdminEmail, "Demo Admin", ApplicationRoles.Admin);
        var trainerUser = await EnsureUserAsync(DemoDataSeedIds.TrainerEmail, "Demo Trainer", ApplicationRoles.Trainer);
        var studentUser = await EnsureUserAsync(DemoDataSeedIds.StudentEmail, "Demo Learner", ApplicationRoles.Student);

        await EnsureTrainerAsync(trainerUser, now, cancellationToken);
        await EnsureStudentAsync(studentUser, now, cancellationToken);

        var cohortConversation = new Conversation
        {
            Name = "Demo LMS Course Cohort",
            IsGroup = true,
            CreatedById = trainerUser.Id,
            SenderId = trainerUser.Id,
            RecieveId = studentUser.Id
        };
        _context.Conversations.Add(cohortConversation);
        await _context.SaveChangesAsync(cancellationToken);

        _context.ConversationParticipants.AddRange(
            new ConversationParticipant
            {
                ConversationId = cohortConversation.Id,
                UserId = trainerUser.Id,
                Role = ParticipantRoles.Admin,
                JoinedAt = now
            },
            new ConversationParticipant
            {
                ConversationId = cohortConversation.Id,
                UserId = studentUser.Id,
                Role = ParticipantRoles.Member,
                JoinedAt = now
            });

        SeedCourseGraph(adminUser, trainerUser, studentUser, cohortConversation.Id, now);
        await _context.SaveChangesAsync(cancellationToken);

        return Result(created: true);
    }

    private async Task<AppUser> EnsureUserAsync(string email, string fullName, string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new AppUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                Avatar = Defaults.AvatarUrl,
                Status = "Active",
                LastSeen = DateTime.UtcNow
            };

            var created = await _userManager.CreateAsync(user, DemoDataSeedIds.Password);
            if (!created.Succeeded)
            {
                var errors = string.Join(", ", created.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to seed demo user '{email}': {errors}");
            }
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            var roleAdded = await _userManager.AddToRoleAsync(user, role);
            if (!roleAdded.Succeeded)
            {
                var errors = string.Join(", ", roleAdded.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to assign role '{role}' to '{email}': {errors}");
            }
        }

        return user;
    }

    private async Task EnsureTrainerAsync(AppUser trainerUser, DateTime now, CancellationToken cancellationToken)
    {
        if (await _context.Trainers.AnyAsync(t => t.TrainerId == DemoDataSeedIds.TrainerId, cancellationToken))
        {
            return;
        }

        _context.Trainers.Add(new Trainer
        {
            TrainerId = DemoDataSeedIds.TrainerId,
            UserId = trainerUser.Id,
            Name = trainerUser.FullName,
            Email = trainerUser.Email,
            Certification = "NASM Certified Personal Trainer",
            Category = "Strength and Conditioning",
            Bio = "Demo instructor account for LMS QA.",
            Description = "Builds structured strength programs with measurable learner outcomes.",
            Experience = 8,
            Rating = 4.8m,
            ApprovalStatus = TrainerApprovalStatus.Approved,
            BankBin = "970436",
            BankAccountNo = "1234567890",
            BankAccountName = "DEMO TRAINER",
            CreatedAt = now,
            UpdatedAt = now
        });
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureStudentAsync(AppUser studentUser, DateTime now, CancellationToken cancellationToken)
    {
        if (await _context.Students.AnyAsync(s => s.StudentId == DemoDataSeedIds.StudentId, cancellationToken))
        {
            return;
        }

        _context.Students.Add(new Student
        {
            StudentId = DemoDataSeedIds.StudentId,
            UserId = studentUser.Id,
            Name = studentUser.FullName,
            Email = studentUser.Email,
            HealthStatus = "Cleared for moderate exercise",
            ProfilePicture = Defaults.StudentAvatarUrl,
            CreatedAt = now,
            UpdatedAt = now
        });
        await _context.SaveChangesAsync(cancellationToken);
    }

    private void SeedCourseGraph(AppUser adminUser, AppUser trainerUser, AppUser studentUser, int cohortConversationId, DateTime now)
    {
        var course = new Course
        {
            CourseId = DemoDataSeedIds.CourseId,
            TrainerId = DemoDataSeedIds.TrainerId,
            Title = "Demo LMS QA Course",
            Description = "A complete sample course for validating learner, trainer, and admin LMS workflows.",
            Type = "Online",
            Category = "Strength Training",
            Price = 149m,
            AdditionalPrice = 25m,
            StartDate = now.Date.AddDays(-7),
            EndDate = now.Date.AddDays(35),
            Duration = 6,
            MaxParticipants = 30,
            Rating = 4.7m,
            Status = CourseStatus.Published
        };

        var option = new CourseOption
        {
            OptionId = "DEMO-OPTION-COACHING",
            CourseId = course.CourseId,
            OptionName = "Weekly coaching check-in",
            Description = "Adds a weekly private coaching review.",
            Price = 25m
        };

        var registration = new CourseRegistration
        {
            RegistrationId = "DEMO-REGISTRATION-001",
            CourseId = course.CourseId,
            StudentId = DemoDataSeedIds.StudentId,
            RegistrationType = "FullCourse",
            Mode = "Online",
            Status = "Active",
            PaymentStatus = PaymentStatus.Paid,
            InitialPayment = course.Price,
            AdditionalFeaturesPayment = option.Price,
            ContractAgreement = "Accepted",
            CreatedAt = now.AddDays(-5),
            UpdatedAt = now.AddDays(-5)
        };

        var payment = new Payment
        {
            PaymentId = "DEMO-PAYMENT-001",
            CourseId = course.CourseId,
            StudentId = DemoDataSeedIds.StudentId,
            PaymentAmount = course.Price + option.Price,
            PaymentDate = now.AddDays(-5),
            PaymentStatus = PaymentStatus.Paid,
            PaymentType = PaymentType.BankTransfer,
            Note = "Seeded paid enrollment for QA.",
            CreatedAt = now.AddDays(-5),
            UpdatedAt = now.AddDays(-5)
        };

        var fundamentals = new GradeCategory
        {
            CategoryId = "DEMO-GRADE-ASSIGNMENTS",
            CourseId = course.CourseId,
            Name = "Assignments",
            WeightPercent = 40m,
            Order = 1,
            IsDefault = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        var quizzes = new GradeCategory
        {
            CategoryId = "DEMO-GRADE-QUIZZES",
            CourseId = course.CourseId,
            Name = "Quizzes",
            WeightPercent = 60m,
            Order = 2,
            IsDefault = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        var module = new CourseModule
        {
            ModuleId = "DEMO-MODULE-FOUNDATIONS",
            CourseId = course.CourseId,
            Title = "Foundations",
            Description = "Baseline movement, safety, and programming principles.",
            Order = 1,
            IsPublished = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        var lectureOne = new Lecture
        {
            LectureId = "DEMO-LECTURE-INTRO",
            CourseId = course.CourseId,
            ModuleId = module.ModuleId,
            Title = "Course orientation",
            Description = "How to complete the course and submit weekly work.",
            ActivityType = LearningActivityType.Lesson,
            Order = 1,
            Duration = 12,
            IsPreview = true,
            IsPublished = true
        };
        var lectureTwo = new Lecture
        {
            LectureId = "DEMO-LECTURE-SQUAT",
            CourseId = course.CourseId,
            ModuleId = module.ModuleId,
            Title = "Squat pattern assessment",
            Description = "Identify range, control, and common compensation patterns.",
            ActivityType = LearningActivityType.Practice,
            Order = 2,
            Duration = 24,
            IsPublished = true
        };

        var assignment = new CourseAssignment
        {
            AssignmentId = "DEMO-ASSIGNMENT-001",
            CourseId = course.CourseId,
            GradeCategoryId = fundamentals.CategoryId,
            Title = "Movement screen reflection",
            Instructions = "Submit a short reflection on the movement screen and your first training target.",
            PointsPossible = 100m,
            DueAt = now.AddDays(3),
            SubmissionType = AssignmentSubmissionType.Text,
            Status = AssignmentStatus.Published,
            CreatedAt = now.AddDays(-4),
            UpdatedAt = now.AddDays(-4)
        };
        var criterionOne = new AssignmentRubricCriterion
        {
            CriterionId = "DEMO-RUBRIC-CLARITY",
            AssignmentId = assignment.AssignmentId,
            Title = "Reflection clarity",
            Description = "Connects assessment observations to a specific training goal.",
            PointsPossible = 50m,
            Order = 1,
            CreatedAt = now,
            UpdatedAt = now
        };
        var criterionTwo = new AssignmentRubricCriterion
        {
            CriterionId = "DEMO-RUBRIC-ACTION",
            AssignmentId = assignment.AssignmentId,
            Title = "Action plan",
            Description = "Defines realistic next steps.",
            PointsPossible = 50m,
            Order = 2,
            CreatedAt = now,
            UpdatedAt = now
        };
        var submission = new AssignmentSubmission
        {
            SubmissionId = "DEMO-SUBMISSION-001",
            AssignmentId = assignment.AssignmentId,
            StudentId = DemoDataSeedIds.StudentId,
            TextResponse = "My primary target is squat depth with better trunk control.",
            Score = 92m,
            ScorePercent = 92m,
            Status = AssignmentSubmissionStatus.Graded,
            Feedback = "Clear reflection with an actionable training focus.",
            SimilarityScorePercent = 8m,
            SimilarityFlags = "Low similarity; no manual review needed.",
            SimilarityCheckedAt = now.AddDays(-1),
            SubmittedAt = now.AddDays(-2),
            GradedAt = now.AddDays(-1),
            UpdatedAt = now.AddDays(-1)
        };

        var quiz = new CourseQuiz
        {
            QuizId = "DEMO-QUIZ-001",
            CourseId = course.CourseId,
            GradeCategoryId = quizzes.CategoryId,
            Title = "Foundations check",
            Description = "Validates the first module concepts.",
            ScopeType = AssessmentScopeType.Module,
            ModuleId = module.ModuleId,
            PassingScorePercent = 70,
            TimeLimitMinutes = 20,
            MaxAttempts = 2,
            ShuffleQuestions = true,
            ShuffleOptions = true,
            ShowCorrectAnswers = true,
            RequireHonorCode = true,
            TrackProctoringSignals = true,
            AvailableFrom = now.AddDays(-2),
            AvailableUntil = now.AddDays(10),
            IsPublished = true,
            CreatedAt = now.AddDays(-3),
            UpdatedAt = now.AddDays(-3)
        };
        var question = new QuizQuestion
        {
            QuestionId = "DEMO-QUIZ-Q1",
            QuizId = quiz.QuizId,
            Prompt = "Which action best supports progressive strength training?",
            QuestionType = QuizQuestionType.SingleChoice,
            Order = 1,
            Points = 10,
            Explanation = "Progressive overload should be planned and recoverable.",
            QuestionBank = "Strength Foundations"
        };
        var correctOption = new QuizOption
        {
            OptionId = "DEMO-QUIZ-Q1-A",
            QuestionId = question.QuestionId,
            Text = "Increase load or volume gradually while tracking recovery.",
            IsCorrect = true
        };
        var quizAttempt = new QuizAttempt
        {
            AttemptId = "DEMO-QUIZ-ATTEMPT-001",
            QuizId = quiz.QuizId,
            StudentId = DemoDataSeedIds.StudentId,
            AttemptNumber = 1,
            Score = 10,
            TotalPoints = 10,
            ScorePercent = 100m,
            Passed = true,
            Status = QuizAttemptStatus.Graded,
            StartedAt = now.AddDays(-1).AddMinutes(-18),
            SubmittedAt = now.AddDays(-1),
            QuestionOrderSnapshot = "[\"DEMO-QUIZ-Q1\"]",
            BrowserFingerprint = "demo-browser",
            IpAddress = "127.0.0.1",
            HonorCodeAccepted = true,
            FocusLostCount = 0,
            PasteEventCount = 0,
            FullscreenExitCount = 0,
            ProctoringEventCount = 0,
            SuspiciousActivityScore = 0,
            GradedAt = now.AddDays(-1),
            Feedback = "Excellent."
        };

        var discussion = new CourseDiscussionQuestion
        {
            QuestionId = "DEMO-DISCUSSION-001",
            CourseId = course.CourseId,
            StudentId = DemoDataSeedIds.StudentId,
            Title = "How should I pace the weekly workouts?",
            Body = "I can train three days per week. Should I repeat the mobility block each time?",
            Status = DiscussionQuestionStatus.Answered,
            IsPinned = true,
            CreatedAt = now.AddDays(-2),
            UpdatedAt = now.AddDays(-1),
            LastActivityAt = now.AddDays(-1)
        };
        var discussionAnswer = new CourseDiscussionAnswer
        {
            AnswerId = "DEMO-DISCUSSION-ANSWER-001",
            QuestionId = discussion.QuestionId,
            AuthorUserId = trainerUser.Id,
            AuthorEntityId = DemoDataSeedIds.TrainerId,
            AuthorRole = DiscussionAuthorRole.Trainer,
            AuthorName = trainerUser.FullName ?? "Demo Trainer",
            AuthorEmail = trainerUser.Email,
            Body = "Use the mobility block as a warm-up before each strength session.",
            IsAccepted = true,
            CreatedAt = now.AddDays(-1),
            UpdatedAt = now.AddDays(-1)
        };
        discussion.AcceptedAnswerId = discussionAnswer.AnswerId;

        _context.Courses.Add(course);
        _context.CourseOptions.Add(option);
        _context.CourseRegistrations.Add(registration);
        _context.CourseRegistrationOptions.Add(new CourseRegistrationOption
        {
            RegistrationOptionId = "DEMO-REGISTRATION-OPTION-001",
            RegistrationId = registration.RegistrationId,
            OptionId = option.OptionId
        });
        _context.Payments.Add(payment);
        _context.PaymentEvents.Add(new PaymentEvent
        {
            PaymentEventId = "DEMO-PAYMENT-EVENT-001",
            PaymentId = payment.PaymentId,
            EventType = PaymentEventType.ManualApproved,
            OldStatus = PaymentStatus.AwaitingConfirmation,
            NewStatus = PaymentStatus.Paid,
            Source = PaymentEventSource.Trainer,
            Message = "Demo payment approved for QA.",
            CreatedAt = now.AddDays(-5)
        });
        _context.GradeCategories.AddRange(fundamentals, quizzes);
        _context.CourseModules.Add(module);
        _context.Lectures.AddRange(lectureOne, lectureTwo);
        _context.LectureMaterials.AddRange(
            new LectureMaterial
            {
                MaterialId = "DEMO-MATERIAL-INTRO",
                LectureId = lectureOne.LectureId,
                MaterialType = "video",
                MaterialContent = "https://example.com/demo/orientation.mp4"
            },
            new LectureMaterial
            {
                MaterialId = "DEMO-MATERIAL-SQUAT",
                LectureId = lectureTwo.LectureId,
                MaterialType = "link",
                MaterialContent = "https://example.com/demo/squat-assessment"
            });
        _context.LectureProgresses.AddRange(
            new LectureProgress
            {
                LectureProgressId = "DEMO-PROGRESS-INTRO",
                StudentId = DemoDataSeedIds.StudentId,
                LectureId = lectureOne.LectureId,
                IsCompleted = true,
                CompletedAt = now.AddDays(-4),
                CreatedAt = now.AddDays(-4),
                UpdatedAt = now.AddDays(-4)
            },
            new LectureProgress
            {
                LectureProgressId = "DEMO-PROGRESS-SQUAT",
                StudentId = DemoDataSeedIds.StudentId,
                LectureId = lectureTwo.LectureId,
                IsCompleted = false,
                CreatedAt = now.AddDays(-3),
                UpdatedAt = now.AddDays(-3)
            });

        _context.CourseAssignments.Add(assignment);
        _context.AssignmentRubricCriteria.AddRange(criterionOne, criterionTwo);
        _context.AssignmentSubmissions.Add(submission);
        _context.AssignmentRubricScores.AddRange(
            new AssignmentRubricScore
            {
                RubricScoreId = "DEMO-RUBRIC-SCORE-CLARITY",
                SubmissionId = submission.SubmissionId,
                CriterionId = criterionOne.CriterionId,
                Score = 46m,
                Feedback = "Strong connection to the screen findings.",
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddDays(-1)
            },
            new AssignmentRubricScore
            {
                RubricScoreId = "DEMO-RUBRIC-SCORE-ACTION",
                SubmissionId = submission.SubmissionId,
                CriterionId = criterionTwo.CriterionId,
                Score = 46m,
                Feedback = "Action plan is practical.",
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddDays(-1)
            });
        _context.AssignmentFeedbackEntries.Add(new AssignmentFeedbackEntry
        {
            FeedbackEntryId = "DEMO-FEEDBACK-001",
            SubmissionId = submission.SubmissionId,
            AuthorUserId = trainerUser.Id,
            AuthorName = trainerUser.FullName ?? "Demo Trainer",
            AuthorRole = DiscussionAuthorRole.Trainer,
            Action = "Graded",
            Status = AssignmentSubmissionStatus.Graded,
            Score = 92m,
            ScorePercent = 92m,
            Feedback = "Clear reflection with an actionable training focus.",
            CreatedAt = now.AddDays(-1)
        });

        _context.CourseQuizzes.Add(quiz);
        _context.QuizQuestions.Add(question);
        _context.QuizOptions.AddRange(
            correctOption,
            new QuizOption
            {
                OptionId = "DEMO-QUIZ-Q1-B",
                QuestionId = question.QuestionId,
                Text = "Change exercises every workout without tracking results.",
                IsCorrect = false
            });
        _context.QuizAttempts.Add(quizAttempt);
        _context.QuizAttemptAnswers.Add(new QuizAttemptAnswer
        {
            AttemptAnswerId = "DEMO-QUIZ-ANSWER-001",
            AttemptId = quizAttempt.AttemptId,
            QuestionId = question.QuestionId,
            SelectedOptionId = correctOption.OptionId,
            IsCorrect = true,
            PointsAwarded = 10,
            Feedback = "Correct."
        });

        _context.CourseDiscussionQuestions.Add(discussion);
        _context.CourseDiscussionAnswers.Add(discussionAnswer);
        _context.CourseStudyGroups.Add(new CourseStudyGroup
        {
            StudyGroupId = "DEMO-STUDY-GROUP-001",
            CourseId = course.CourseId,
            ConversationId = cohortConversationId,
            Name = "Demo LMS Course Cohort",
            Description = "Default learner cohort for course chat and moderation QA.",
            Kind = CourseStudyGroupKind.Cohort,
            IsDefaultCohort = true,
            IsActive = true,
            CreatedByUserId = trainerUser.Id,
            CreatedAt = now,
            UpdatedAt = now
        });

        _context.CourseAnnouncements.Add(new CourseAnnouncement
        {
            AnnouncementId = "DEMO-ANNOUNCEMENT-001",
            CourseId = course.CourseId,
            CreatedByUserId = trainerUser.Id,
            CreatedByRole = DiscussionAuthorRole.Trainer,
            CreatedByName = trainerUser.FullName ?? "Demo Trainer",
            Title = "Welcome to the demo LMS course",
            Body = "Use this course to validate learner, instructor, and admin workflows.",
            IsPinned = true,
            IsPublished = true,
            PublishedAt = now.AddDays(-3),
            CreatedAt = now.AddDays(-3),
            UpdatedAt = now.AddDays(-3)
        });
        _context.CourseLiveSessions.Add(new CourseLiveSession
        {
            LiveSessionId = "DEMO-LIVE-SESSION-001",
            CourseId = course.CourseId,
            Title = "Live form review",
            Description = "Trainer-led review of submitted movement screens.",
            StartsAt = now.AddDays(2),
            EndsAt = now.AddDays(2).AddHours(1),
            MeetingUrl = "https://example.com/demo/live-review",
            Status = CourseLiveSessionStatus.Scheduled,
            AttendanceRequired = true,
            CreatedAt = now,
            UpdatedAt = now,
            PublishedAt = now
        });
        _context.CourseCertificateSettings.Add(new CourseCertificateSetting
        {
            CourseId = course.CourseId,
            IsEnabled = true,
            TemplateName = "Modern",
            CertificateTitle = "Demo Strength Foundations Certificate",
            BodyText = "Awarded for completing the demo LMS QA course.",
            AccentColor = "#1f7a5a",
            RequiredLecturePercent = 80m,
            RequirePublishedQuizzes = true,
            RequirePublishedAssignments = true,
            RequiredAssignmentPercent = 70m,
            MinimumFinalGradePercent = 70m,
            CreatedAt = now,
            UpdatedAt = now
        });
        _context.CourseCertificates.Add(new CourseCertificate
        {
            CertificateId = "DEMO-CERTIFICATE-001",
            CourseId = course.CourseId,
            StudentId = DemoDataSeedIds.StudentId,
            VerificationCode = "DEMO-CERT-VERIFY",
            IssuedAt = now
        });
        _context.Notifications.AddRange(
            new Notification
            {
                UserId = studentUser.Id,
                Type = NotificationTypes.Announcement,
                Title = "New course announcement",
                Content = "Welcome to the demo LMS course",
                Link = $"/client/course-announcements/{course.CourseId}",
                IsRead = false,
                CreatedAt = now.AddDays(-3)
            },
            new Notification
            {
                UserId = trainerUser.Id,
                Type = NotificationTypes.Grading,
                Title = "Demo submission graded",
                Content = "The seeded learner has a graded assignment for gradebook QA.",
                Link = $"/agency/assignments/{course.CourseId}",
                IsRead = true,
                CreatedAt = now.AddDays(-1)
            },
            new Notification
            {
                UserId = adminUser.Id,
                Type = NotificationTypes.System,
                Title = "Demo LMS dataset ready",
                Content = "Seeded demo data is available for end-to-end QA.",
                Link = "/admin/courses",
                IsRead = false,
                CreatedAt = now
            });
    }

    private static DemoDataSeedResult Result(bool created) =>
        new(created, DemoDataSeedIds.CourseId, DemoDataSeedIds.TrainerEmail, DemoDataSeedIds.StudentEmail);
}

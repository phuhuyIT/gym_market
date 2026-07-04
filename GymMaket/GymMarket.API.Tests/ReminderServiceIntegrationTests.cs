using System.Net.Http.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.ClassSchedule;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.Membership;
using GymMarket.API.DTOs.Notifications;
using GymMarket.API.DTOs.Workout;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class ReminderServiceIntegrationTests : BaseIntegrationTests
{
    public ReminderServiceIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task SendDueRemindersAsync_ForUpcomingBookedClass_NotifiesStudentOnce()
    {
        var plan = await CreateMembershipPlan("Reminder Class Access", 30);
        var session = await CreateClassSession("Morning Reminder", DateTime.UtcNow.AddHours(6));

        await AuthenticateAsync(email: "reminder-class-student@example.com", role: "Student");
        await Subscribe(plan.PlanId);
        var booking = await Client.PostAsync($"/api/ClassSchedule/sessions/{session.ClassSessionId}/book", null);
        booking.EnsureSuccessStatusCode();

        var firstRun = await RunReminders();
        var secondRun = await RunReminders();

        Assert.True(firstRun >= 1);
        Assert.Equal(0, secondRun);

        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.Single(notifications!, n =>
            n.Type == NotificationTypes.Class
            && n.Title == "Class starts soon"
            && n.Link == "/client/classes"
            && n.Content!.Contains(session.Title));
    }

    [Fact]
    public async Task SendDueRemindersAsync_ForMembershipExpiringSoon_NotifiesStudent()
    {
        var plan = await CreateMembershipPlan("Reminder Seven Day Access", 7);
        await AuthenticateAsync(email: "reminder-membership-student@example.com", role: "Student");
        await Subscribe(plan.PlanId);

        await RunReminders();

        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.Contains(notifications!, n =>
            n.Type == NotificationTypes.Membership
            && n.Title == "Membership expiring soon"
            && n.Link == "/client/membership"
            && n.Content!.Contains(plan.Name));
    }

    [Fact]
    public async Task SendDueRemindersAsync_ForInactiveWorkoutAndMissingProgress_NotifiesStudent()
    {
        var studentEmail = "reminder-inactive-student@example.com";
        var trainerEmail = "reminder-inactive-trainer@example.com";
        var studentId = await CreateStudent(studentEmail);
        var plan = await CreateWorkoutPlanAsTrainer(trainerEmail, "Reminder Strength Plan");

        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var assignResponse = await Client.PostAsJsonAsync($"/api/WorkoutPlans/plans/{plan.WorkoutPlanId}/assign", new AssignWorkoutPlanDto
        {
            StudentId = studentId
        });
        assignResponse.EnsureSuccessStatusCode();
        var assignment = (await assignResponse.Content.ReadFromJsonAsync<StudentWorkoutAssignmentDto>())!;

        await AgeAssignment(assignment.AssignmentId, DateTime.UtcNow.AddDays(-8));

        await RunReminders();

        await AuthenticateAsync(email: studentEmail, role: "Student");
        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.Contains(notifications!, n =>
            n.Type == NotificationTypes.Progress
            && n.Title == "Progress check-in due"
            && n.Link == "/client/progress");
        Assert.Contains(notifications!, n =>
            n.Type == NotificationTypes.Workout
            && n.Title == "Workout waiting"
            && n.Link == "/client/workouts"
            && n.Content!.Contains(plan.Name));
    }

    [Fact]
    public async Task SendDueRemindersAsync_ForCourseScheduleItems_NotifiesPaidStudent()
    {
        var trainerEmail = "reminder-course-trainer@example.com";
        var studentEmail = "reminder-course-student@example.com";
        var courseId = await CreateCourseAsync(trainerEmail);
        await AuthenticateAsync(email: studentEmail, role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentId, courseId);
        await SeedCourseScheduleItemsAsync(courseId, DateTime.UtcNow.AddHours(12));

        var sent = await RunReminders();

        Assert.True(sent >= 3);
        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.Contains(notifications!, n =>
            n.Type == NotificationTypes.Assignment
            && n.Title == "Assignment due soon"
            && n.Link == $"/client/course-assignments/{courseId}");
        Assert.Contains(notifications!, n =>
            n.Type == NotificationTypes.Quiz
            && n.Title == "Quiz closes soon"
            && n.Link == $"/client/course-learn/{courseId}");
        Assert.Contains(notifications!, n =>
            n.Type == NotificationTypes.LiveSession
            && n.Title == "Live session starts soon"
            && n.Link == $"/client/course-live-sessions/{courseId}");
    }

    private async Task<int> RunReminders()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IReminderService>();
        return await service.SendDueRemindersAsync();
    }

    private async Task AgeAssignment(string assignmentId, DateTime createdAt)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var assignment = await db.StudentWorkoutAssignments.FirstAsync(a => a.AssignmentId == assignmentId);
        assignment.CreatedAt = createdAt;
        assignment.StartsAt = createdAt;
        await db.SaveChangesAsync();
    }

    private async Task<string> CreateStudent(string email)
    {
        await AuthenticateAsync(email: email, role: "Student");
        return GetTokenClaim("studentId")!;
    }

    private async Task Subscribe(string planId)
    {
        var response = await Client.PostAsJsonAsync("/api/Memberships/subscribe", new SubscribeMembershipDto
        {
            PlanId = planId
        });

        response.EnsureSuccessStatusCode();
    }

    private async Task<MembershipPlanDto> CreateMembershipPlan(string name, int durationDays)
    {
        await AuthenticateAsAdminAsync(email: $"reminder-membership-admin-{Guid.NewGuid():N}@example.com");
        var response = await Client.PostAsJsonAsync("/api/Memberships/plans", new UpsertMembershipPlanDto
        {
            Name = name,
            DurationDays = durationDays,
            Price = 29,
            IsActive = true
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<MembershipPlanDto>())!;
    }

    private async Task<ClassSessionDto> CreateClassSession(string title, DateTime startsAt)
    {
        await AuthenticateAsAdminAsync(email: $"reminder-class-admin-{Guid.NewGuid():N}@example.com");
        var response = await Client.PostAsJsonAsync("/api/ClassSchedule/sessions", new UpsertClassSessionDto
        {
            Title = title,
            Description = "Automated reminder test",
            StartsAt = startsAt,
            EndsAt = startsAt.AddHours(1),
            Capacity = 8,
            Location = "Studio R",
            Status = ClassSessionStatus.Scheduled
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClassSessionDto>())!;
    }

    private async Task<WorkoutPlanDto> CreateWorkoutPlanAsTrainer(string trainerEmail, string name)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var response = await Client.PostAsJsonAsync("/api/WorkoutPlans/plans", new UpsertWorkoutPlanDto
        {
            Name = name,
            Goal = "Rebuild consistency",
            Difficulty = "Beginner",
            DurationWeeks = 4,
            IsActive = true,
            Exercises =
            [
                new UpsertWorkoutExerciseDto
                {
                    WeekNumber = 1,
                    DayNumber = 1,
                    Order = 1,
                    Name = "Squat",
                    Sets = 3,
                    Reps = "10",
                    RestSeconds = 60
                }
            ]
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<WorkoutPlanDto>())!;
    }

    private async Task<string> CreateCourseAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "REM_COURSE_" + Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = "Reminder course " + courseId,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(30),
            Price = 100,
            MaxParticipants = 10,
            Duration = 10,
            Status = CourseStatus.PendingReview
        });
        response.EnsureSuccessStatusCode();
        return courseId;
    }

    private async Task SeedPaidPaymentAsync(string studentId, string courseId)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        db.Payments.Add(new Payment
        {
            PaymentId = Guid.NewGuid().ToString(),
            StudentId = studentId,
            CourseId = courseId,
            PaymentAmount = 100,
            PaymentStatus = PaymentStatus.Paid,
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    private async Task SeedCourseScheduleItemsAsync(string courseId, DateTime startsAt)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        db.CourseAssignments.Add(new CourseAssignment
        {
            AssignmentId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            Title = "Reminder assignment",
            Instructions = "Submit before the deadline.",
            DueAt = startsAt,
            Status = AssignmentStatus.Published,
            PointsPossible = 100,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        db.CourseQuizzes.Add(new CourseQuiz
        {
            QuizId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            Title = "Reminder quiz",
            Description = "Closing soon.",
            AvailableUntil = startsAt,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        db.CourseLiveSessions.Add(new CourseLiveSession
        {
            LiveSessionId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            Title = "Reminder live session",
            StartsAt = startsAt,
            EndsAt = startsAt.AddHours(1),
            Status = CourseLiveSessionStatus.Scheduled,
            AttendanceRequired = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PublishedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }
}

using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Notifications;
using GymMarket.API.DTOs.Progress;
using GymMarket.API.DTOs.Workout;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class ProgressIntegrationTests : BaseIntegrationTests
{
    public ProgressIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateLog_AsStudent_ReturnsOwnProgressLog()
    {
        await AuthenticateAsync(email: "progress-log-student@example.com", role: "Student");

        var response = await Client.PostAsJsonAsync("/api/Progress/me/logs", new UpsertProgressLogDto
        {
            WeightKg = 82.4m,
            BodyFatPercent = 21.5m,
            WaistCm = 88,
            StrengthNotes = "Squat felt stronger"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var log = await response.Content.ReadFromJsonAsync<ProgressLogDto>();
        Assert.NotNull(log);
        Assert.Equal(82.4m, log!.WeightKg);

        var logs = await Client.GetFromJsonAsync<List<ProgressLogDto>>("/api/Progress/me/logs");
        Assert.Single(logs!);
    }

    [Fact]
    public async Task UpsertGoal_AsStudent_ReturnsActiveGoal()
    {
        await AuthenticateAsync(email: "progress-goal-student@example.com", role: "Student");

        var response = await Client.PutAsJsonAsync("/api/Progress/me/goal", new UpsertProgressGoalDto
        {
            TargetWeightKg = 75,
            TargetBodyFatPercent = 16,
            GoalDate = DateTime.UtcNow.AddDays(90),
            Notes = "Cut phase"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var goal = await response.Content.ReadFromJsonAsync<ProgressGoalDto>();
        Assert.NotNull(goal);
        Assert.Equal("Active", goal!.Status);
        Assert.Equal(75, goal.TargetWeightKg);
    }

    [Fact]
    public async Task GetSummaries_AsTrainer_ReturnsAssignedStudentsOnly()
    {
        var assignedStudentId = await CreateStudent("progress-assigned-student@example.com");
        var otherStudentId = await CreateStudent("progress-other-student@example.com");
        var plan = await CreatePlanAsTrainer("progress-review-trainer@example.com");

        await AuthenticateAsync(email: "progress-review-trainer@example.com", role: "Trainer");
        var assignResponse = await Client.PostAsJsonAsync($"/api/WorkoutPlans/plans/{plan.WorkoutPlanId}/assign", new AssignWorkoutPlanDto
        {
            StudentId = assignedStudentId
        });
        assignResponse.EnsureSuccessStatusCode();

        await CreateProgressLog("progress-assigned-student@example.com", 80, 20, DateTime.UtcNow.AddDays(-7));
        await CreateProgressLog("progress-assigned-student@example.com", 78, 18.5m, DateTime.UtcNow);
        await CreateProgressLog("progress-other-student@example.com", 90, 25);

        await AuthenticateAsync(email: "progress-review-trainer@example.com", role: "Trainer");
        var summaries = await Client.GetFromJsonAsync<List<ProgressSummaryDto>>("/api/Progress/summaries");

        Assert.NotNull(summaries);
        Assert.Contains(summaries!, s => s.StudentId == assignedStudentId && s.WeightChangeKg == -2);
        Assert.DoesNotContain(summaries!, s => s.StudentId == otherStudentId);
    }

    [Fact]
    public async Task GetStudentLogs_UnassignedTrainer_ReturnsForbidden()
    {
        var studentId = await CreateStudent("progress-private-student@example.com");
        await CreateProgressLog("progress-private-student@example.com", 70, 19);

        await AuthenticateAsync(email: "progress-unassigned-trainer@example.com", role: "Trainer");
        var response = await Client.GetAsync($"/api/Progress/students/{studentId}/logs");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetStudentLogs_AssignedTrainer_ReturnsTimeline()
    {
        var studentId = await CreateStudent("progress-timeline-student@example.com");
        var plan = await CreatePlanAsTrainer("progress-timeline-trainer@example.com");
        await AuthenticateAsync(email: "progress-timeline-trainer@example.com", role: "Trainer");
        var assignResponse = await Client.PostAsJsonAsync($"/api/WorkoutPlans/plans/{plan.WorkoutPlanId}/assign", new AssignWorkoutPlanDto
        {
            StudentId = studentId
        });
        assignResponse.EnsureSuccessStatusCode();
        await CreateProgressLog("progress-timeline-student@example.com", 76, 17);

        await AuthenticateAsync(email: "progress-timeline-trainer@example.com", role: "Trainer");
        var logs = await Client.GetFromJsonAsync<List<ProgressLogDto>>($"/api/Progress/students/{studentId}/logs");

        Assert.Single(logs!);
        Assert.Equal(76, logs![0].WeightKg);

        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.Contains(notifications!, n =>
            n.Type == "progress"
            && n.Title == "Progress log added"
            && n.Link == "/agency/progress");
    }

    private async Task<string> CreateStudent(string email)
    {
        await AuthenticateAsync(email: email, role: "Student");
        return GetTokenClaim("studentId")!;
    }

    private async Task CreateProgressLog(string email, decimal weightKg, decimal bodyFatPercent, DateTime? loggedAt = null)
    {
        await AuthenticateAsync(email: email, role: "Student");
        var response = await Client.PostAsJsonAsync("/api/Progress/me/logs", new UpsertProgressLogDto
        {
            WeightKg = weightKg,
            BodyFatPercent = bodyFatPercent,
            LoggedAt = loggedAt ?? DateTime.UtcNow
        });
        response.EnsureSuccessStatusCode();
    }

    private async Task<WorkoutPlanDto> CreatePlanAsTrainer(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var response = await Client.PostAsJsonAsync("/api/WorkoutPlans/plans", new UpsertWorkoutPlanDto
        {
            Name = "Progress Review Plan",
            Goal = "Create trainer review access",
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
}

using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Notifications;
using GymMarket.API.DTOs.Workout;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class WorkoutPlansIntegrationTests : BaseIntegrationTests
{
    public WorkoutPlansIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreatePlan_AsStudent_ReturnsForbidden()
    {
        await AuthenticateAsync(email: "workout-student-forbidden@example.com", role: "Student");

        var response = await Client.PostAsJsonAsync("/api/WorkoutPlans/plans", SamplePlan("Student Plan"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task TrainerCreatesPlan_ThenStudentReceivesAssignment()
    {
        var studentId = await CreateStudent("workout-assigned-student@example.com");
        var plan = await CreatePlanAsTrainer("workout-owner@example.com", "Strength Base");

        await AuthenticateAsync(email: "workout-owner@example.com", role: "Trainer");
        var response = await Client.PostAsJsonAsync($"/api/WorkoutPlans/plans/{plan.WorkoutPlanId}/assign", new AssignWorkoutPlanDto
        {
            StudentId = studentId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var assignment = await response.Content.ReadFromJsonAsync<StudentWorkoutAssignmentDto>();
        Assert.NotNull(assignment);
        Assert.Equal(plan.WorkoutPlanId, assignment!.WorkoutPlanId);
        Assert.Equal(studentId, assignment.StudentId);
        Assert.Equal("Active", assignment.Status);
        Assert.Equal(2, assignment.TotalExercises);

        await AuthenticateAsync(email: "workout-assigned-student@example.com", role: "Student");
        var mine = await Client.GetFromJsonAsync<List<StudentWorkoutAssignmentDto>>("/api/WorkoutPlans/my-assignments");
        Assert.Contains(mine!, a => a.AssignmentId == assignment.AssignmentId);

        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.Contains(notifications!, n =>
            n.Type == "workout"
            && n.Title == "Workout plan assigned"
            && n.Link == "/client/workouts"
            && n.Content!.Contains(plan.Name));
    }

    [Fact]
    public async Task CompleteExercise_WhenAllExercisesDone_CompletesAssignment()
    {
        var studentId = await CreateStudent("workout-complete-student@example.com");
        var plan = await CreatePlanAsTrainer("workout-complete-trainer@example.com", "Finish Plan");

        await AuthenticateAsync(email: "workout-complete-trainer@example.com", role: "Trainer");
        var assignResponse = await Client.PostAsJsonAsync($"/api/WorkoutPlans/plans/{plan.WorkoutPlanId}/assign", new AssignWorkoutPlanDto
        {
            StudentId = studentId
        });
        assignResponse.EnsureSuccessStatusCode();
        var assignment = (await assignResponse.Content.ReadFromJsonAsync<StudentWorkoutAssignmentDto>())!;

        await AuthenticateAsync(email: "workout-complete-student@example.com", role: "Student");
        var first = await Client.PostAsJsonAsync(
            $"/api/WorkoutPlans/assignments/{assignment.AssignmentId}/exercises/{assignment.Exercises[0].ExerciseId}/complete",
            new CompleteWorkoutExerciseDto { Notes = "Done" });
        first.EnsureSuccessStatusCode();
        var partial = (await first.Content.ReadFromJsonAsync<StudentWorkoutAssignmentDto>())!;
        Assert.Equal("Active", partial.Status);
        Assert.Equal(1, partial.CompletedExercises);

        var second = await Client.PostAsJsonAsync(
            $"/api/WorkoutPlans/assignments/{assignment.AssignmentId}/exercises/{assignment.Exercises[1].ExerciseId}/complete",
            new CompleteWorkoutExerciseDto());
        second.EnsureSuccessStatusCode();
        var completed = (await second.Content.ReadFromJsonAsync<StudentWorkoutAssignmentDto>())!;

        Assert.Equal("Completed", completed.Status);
        Assert.Equal(2, completed.CompletedExercises);
        Assert.Equal(100, completed.CompletionPercent);
        Assert.NotNull(completed.CompletedAt);

        await AuthenticateAsync(email: "workout-complete-trainer@example.com", role: "Trainer");
        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.Contains(notifications!, n =>
            n.Type == "workout"
            && n.Title == "Workout completed"
            && n.Link == "/agency/workouts"
            && n.Content!.Contains("Finish Plan"));
    }

    [Fact]
    public async Task AssignPlan_OtherTrainerPlan_ReturnsForbidden()
    {
        var studentId = await CreateStudent("workout-forbidden-student@example.com");
        var plan = await CreatePlanAsTrainer("workout-first-trainer@example.com", "Owned Plan");

        await AuthenticateAsync(email: "workout-second-trainer@example.com", role: "Trainer");
        var response = await Client.PostAsJsonAsync($"/api/WorkoutPlans/plans/{plan.WorkoutPlanId}/assign", new AssignWorkoutPlanDto
        {
            StudentId = studentId
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAssignments_AsTrainer_OnlyReturnsOwnAssignments()
    {
        var firstStudentId = await CreateStudent("workout-list-first-student@example.com");
        var secondStudentId = await CreateStudent("workout-list-second-student@example.com");
        var firstPlan = await CreatePlanAsTrainer("workout-list-first-trainer@example.com", "First Trainer Plan");
        var secondPlan = await CreatePlanAsTrainer("workout-list-second-trainer@example.com", "Second Trainer Plan");

        await AuthenticateAsync(email: "workout-list-first-trainer@example.com", role: "Trainer");
        var firstAssign = await Client.PostAsJsonAsync($"/api/WorkoutPlans/plans/{firstPlan.WorkoutPlanId}/assign", new AssignWorkoutPlanDto
        {
            StudentId = firstStudentId
        });
        firstAssign.EnsureSuccessStatusCode();

        await AuthenticateAsync(email: "workout-list-second-trainer@example.com", role: "Trainer");
        var secondAssign = await Client.PostAsJsonAsync($"/api/WorkoutPlans/plans/{secondPlan.WorkoutPlanId}/assign", new AssignWorkoutPlanDto
        {
            StudentId = secondStudentId
        });
        secondAssign.EnsureSuccessStatusCode();

        await AuthenticateAsync(email: "workout-list-first-trainer@example.com", role: "Trainer");
        var assignments = await Client.GetFromJsonAsync<List<StudentWorkoutAssignmentDto>>("/api/WorkoutPlans/assignments");

        Assert.NotNull(assignments);
        Assert.Contains(assignments!, a => a.WorkoutPlanId == firstPlan.WorkoutPlanId);
        Assert.DoesNotContain(assignments!, a => a.WorkoutPlanId == secondPlan.WorkoutPlanId);
    }

    private async Task<string> CreateStudent(string email)
    {
        await AuthenticateAsync(email: email, role: "Student");
        return GetTokenClaim("studentId")!;
    }

    private async Task<WorkoutPlanDto> CreatePlanAsTrainer(string trainerEmail, string name)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var response = await Client.PostAsJsonAsync("/api/WorkoutPlans/plans", SamplePlan(name));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<WorkoutPlanDto>())!;
    }

    private static UpsertWorkoutPlanDto SamplePlan(string name)
    {
        return new UpsertWorkoutPlanDto
        {
            Name = name,
            Goal = "Build consistent gym habits",
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
                    Name = "Goblet squat",
                    Sets = 3,
                    Reps = "10",
                    RestSeconds = 60,
                    Notes = "Control the descent"
                },
                new UpsertWorkoutExerciseDto
                {
                    WeekNumber = 1,
                    DayNumber = 1,
                    Order = 2,
                    Name = "Push-up",
                    Sets = 3,
                    Reps = "8-12",
                    RestSeconds = 60
                }
            ]
        };
    }
}

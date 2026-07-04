using System.Net;
using System.Net.Http.Json;
using GymMarket.API;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Course;
using GymMarket.API.DTOs.CourseStudyGroups;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class CourseStudyGroupsIntegrationTests : BaseIntegrationTests
{
    public CourseStudyGroupsIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task SyncDefaultCohort_CreatesCourseConversationAndAddsPaidLearners()
    {
        var courseId = await CreateCourseAsync("cohort_trainer@example.com");

        await AuthenticateAsync(email: "cohort_student@example.com", role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        var studentUserId = GetTokenClaim("nameid")!;
        await SeedPaidPaymentAsync(studentId, courseId);

        await AuthenticateAsync(email: "cohort_trainer@example.com", role: "Trainer");
        var trainerUserId = GetTokenClaim("nameid")!;

        var response = await Client.PostAsJsonAsync($"/api/CourseStudyGroups/course/{courseId}/sync", new { });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var cohort = await response.Content.ReadFromJsonAsync<CourseStudyGroupDto>();
        Assert.NotNull(cohort);
        Assert.True(cohort!.IsDefaultCohort);
        Assert.Equal(CourseStudyGroupKind.Cohort, cohort.Kind);
        Assert.True(cohort.ConversationId > 0);
        Assert.Contains(cohort.Members, m => m.UserId == trainerUserId && m.Role == ParticipantRoles.Owner);
        Assert.Contains(cohort.Members, m => m.UserId == studentUserId && m.Role == ParticipantRoles.Member);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        Assert.Single(db.CourseStudyGroups.Where(g => g.CourseId == courseId && g.IsDefaultCohort));
        Assert.True(db.ConversationParticipants.Any(p => p.ConversationId == cohort.ConversationId && p.UserId == studentUserId));
    }

    [Fact]
    public async Task PaidStudent_CanListOwnCourseGroups_ButUnpaidStudentCannot()
    {
        var courseId = await CreateCourseAsync("cohort_access_trainer@example.com");

        await AuthenticateAsync(email: "cohort_access_student@example.com", role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        await SeedPaidPaymentAsync(studentId, courseId);

        await AuthenticateAsync(email: "cohort_access_trainer@example.com", role: "Trainer");
        var sync = await Client.PostAsJsonAsync($"/api/CourseStudyGroups/course/{courseId}/sync", new { });
        sync.EnsureSuccessStatusCode();

        await AuthenticateAsync(email: "cohort_access_student@example.com", role: "Student");
        var groups = await Client.GetFromJsonAsync<List<CourseStudyGroupDto>>($"/api/CourseStudyGroups/course/{courseId}");
        Assert.NotNull(groups);
        Assert.Single(groups!);
        Assert.True(groups[0].IsMember);

        await AuthenticateAsync(email: "cohort_unpaid_student@example.com", role: "Student");
        var forbidden = await Client.GetAsync($"/api/CourseStudyGroups/course/{courseId}");
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
    }

    [Fact]
    public async Task Trainer_CanCreateStudyGroupAndManagePaidLearnerMembership()
    {
        var courseId = await CreateCourseAsync("study_group_trainer@example.com");

        await AuthenticateAsync(email: "study_group_student@example.com", role: "Student");
        var studentId = GetTokenClaim("studentId")!;
        var studentUserId = GetTokenClaim("nameid")!;
        await SeedPaidPaymentAsync(studentId, courseId);

        await AuthenticateAsync(email: "study_group_trainer@example.com", role: "Trainer");

        var create = await Client.PostAsJsonAsync($"/api/CourseStudyGroups/course/{courseId}/groups", new UpsertCourseStudyGroupDto
        {
            Name = "Exam prep group",
            Description = "Learners preparing for the final assessment.",
            Kind = CourseStudyGroupKind.StudyGroup,
            IsActive = true
        });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        var group = await create.Content.ReadFromJsonAsync<CourseStudyGroupDto>();
        Assert.NotNull(group);
        Assert.False(group!.IsDefaultCohort);
        Assert.DoesNotContain(group.Members, m => m.UserId == studentUserId);

        var add = await Client.PostAsJsonAsync($"/api/CourseStudyGroups/groups/{group.StudyGroupId}/members", new ManageCourseStudyGroupMembersDto
        {
            UserIds = [studentUserId]
        });
        Assert.Equal(HttpStatusCode.OK, add.StatusCode);

        var groupsAfterAdd = await Client.GetFromJsonAsync<List<CourseStudyGroupDto>>($"/api/CourseStudyGroups/course/{courseId}/manage");
        var updatedGroup = groupsAfterAdd!.Single(g => g.StudyGroupId == group.StudyGroupId);
        Assert.Contains(updatedGroup.Members, m => m.UserId == studentUserId);

        var promote = await Client.PutAsJsonAsync($"/api/CourseStudyGroups/groups/{group.StudyGroupId}/members/{studentUserId}/role", new UpdateCourseStudyGroupMemberRoleDto
        {
            Role = ParticipantRoles.Admin
        });
        Assert.Equal(HttpStatusCode.OK, promote.StatusCode);

        var remove = await Client.DeleteAsync($"/api/CourseStudyGroups/groups/{group.StudyGroupId}/members/{studentUserId}");
        Assert.Equal(HttpStatusCode.OK, remove.StatusCode);

        var groupsAfterRemove = await Client.GetFromJsonAsync<List<CourseStudyGroupDto>>($"/api/CourseStudyGroups/course/{courseId}/manage");
        updatedGroup = groupsAfterRemove!.Single(g => g.StudyGroupId == group.StudyGroupId);
        Assert.DoesNotContain(updatedGroup.Members, m => m.UserId == studentUserId);
    }

    private async Task<string> CreateCourseAsync(string trainerEmail)
    {
        await AuthenticateAsync(email: trainerEmail, role: "Trainer");
        var courseId = "COHORT_" + Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync("/api/Course", new CourseCreateDTO
        {
            CourseId = courseId,
            Title = "Cohort " + courseId,
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
}

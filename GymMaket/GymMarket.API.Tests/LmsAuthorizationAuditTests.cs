using System.Net;
using System.Net.Http.Json;
using GymMarket.API;
using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class LmsAuthorizationAuditTests : BaseIntegrationTests
{
    public LmsAuthorizationAuditTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task DemoDataSeeder_IsDisabledByDefaultAtStartup()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();

        Assert.False(await db.Courses.AnyAsync(c => c.CourseId == DemoDataSeedIds.CourseId));
    }

    [Fact]
    public async Task CrossRoleAccess_ToPrivateLmsCourseData_IsForbidden()
    {
        var fixture = await CreateAuditFixtureAsync();

        await AuthenticateAsync(email: fixture.OwnerTrainerEmail, role: "Trainer");
        Assert.Equal(HttpStatusCode.OK, (await Client.GetAsync($"/api/Gradebook/course/{fixture.CourseId}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Client.GetAsync($"/api/Assignments/{fixture.AssignmentId}/submissions")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Client.GetAsync($"/api/Certificates/course/{fixture.CourseId}/settings")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Client.GetAsync($"/api/Payments/get-payments-of-course/{fixture.CourseId}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Client.GetAsync($"/api/CourseAnalytics/course/{fixture.CourseId}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Client.GetAsync($"/api/CourseStudyGroups/course/{fixture.CourseId}/manage")).StatusCode);

        await AuthenticateAsync(email: fixture.OtherTrainerEmail, role: "Trainer");
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/Gradebook/course/{fixture.CourseId}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/Gradebook/course/{fixture.CourseId}/export")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/Assignments/{fixture.AssignmentId}/submissions")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/Certificates/course/{fixture.CourseId}/settings")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/Payments/get-payments-of-course/{fixture.CourseId}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/CourseAnalytics/course/{fixture.CourseId}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/CourseStudyGroups/course/{fixture.CourseId}/manage")).StatusCode);

        await AuthenticateAsync(email: fixture.PaidStudentEmail, role: "Student");
        Assert.Equal(HttpStatusCode.OK, (await Client.GetAsync($"/api/Gradebook/course/{fixture.CourseId}/me")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Client.GetAsync($"/api/Assignments/course/{fixture.CourseId}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Client.GetAsync($"/api/Certificates/course/{fixture.CourseId}/completion")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Client.GetAsync($"/api/CourseAnalytics/course/{fixture.CourseId}/me")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Client.GetAsync($"/api/CourseStudyGroups/course/{fixture.CourseId}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/Assignments/{fixture.AssignmentId}/submissions")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/Payments/get-payments-of-course/{fixture.CourseId}")).StatusCode);

        await AuthenticateAsync(email: fixture.UnpaidStudentEmail, role: "Student");
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/Gradebook/course/{fixture.CourseId}/me")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/Assignments/course/{fixture.CourseId}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/Certificates/course/{fixture.CourseId}/completion")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.PostAsJsonAsync($"/api/Certificates/course/{fixture.CourseId}/issue", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/CourseAnalytics/course/{fixture.CourseId}/me")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Client.GetAsync($"/api/CourseStudyGroups/course/{fixture.CourseId}")).StatusCode);
    }

    [Fact]
    public async Task StudentPrivateData_IsScopedToAuthenticatedStudent()
    {
        var fixture = await CreateAuditFixtureAsync();

        await AuthenticateAsync(email: fixture.PaidStudentEmail, role: "Student");
        var ownCertificates = await Client.GetAsync("/api/Certificates/me");
        Assert.Equal(HttpStatusCode.OK, ownCertificates.StatusCode);
        var ownBody = await ownCertificates.Content.ReadAsStringAsync();
        Assert.Contains(fixture.CertificateId, ownBody);

        await AuthenticateAsync(email: fixture.UnpaidStudentEmail, role: "Student");
        var otherCertificates = await Client.GetAsync("/api/Certificates/me");
        Assert.Equal(HttpStatusCode.OK, otherCertificates.StatusCode);
        var otherBody = await otherCertificates.Content.ReadAsStringAsync();
        Assert.DoesNotContain(fixture.CertificateId, otherBody);

        var publicTrainer = await Client.GetAsync($"/api/Trainer/{fixture.OwnerTrainerId}");
        Assert.Equal(HttpStatusCode.OK, publicTrainer.StatusCode);
        var publicTrainerBody = await publicTrainer.Content.ReadAsStringAsync();
        Assert.DoesNotContain("BankAccountNo", publicTrainerBody);
        Assert.DoesNotContain("BankBin", publicTrainerBody);
    }

    private async Task<LmsAuditFixture> CreateAuditFixtureAsync()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var ownerTrainerEmail = $"audit-owner-{suffix}@example.com";
        var otherTrainerEmail = $"audit-other-trainer-{suffix}@example.com";
        var paidStudentEmail = $"audit-paid-student-{suffix}@example.com";
        var unpaidStudentEmail = $"audit-unpaid-student-{suffix}@example.com";

        await AuthenticateAsync(email: ownerTrainerEmail, role: "Trainer");
        var ownerTrainerId = GetTokenClaim("trainerId")!;
        var ownerTrainerUserId = GetTokenClaim("nameid")!;

        await AuthenticateAsync(email: otherTrainerEmail, role: "Trainer");

        await AuthenticateAsync(email: paidStudentEmail, role: "Student");
        var paidStudentId = GetTokenClaim("studentId")!;
        var paidStudentUserId = GetTokenClaim("nameid")!;

        await AuthenticateAsync(email: unpaidStudentEmail, role: "Student");

        var courseId = $"AUDIT_{suffix}";
        var assignmentId = $"AUDIT_ASSIGNMENT_{suffix}";
        var submissionId = $"AUDIT_SUBMISSION_{suffix}";
        var paymentId = $"AUDIT_PAYMENT_{suffix}";
        var certificateId = $"AUDIT_CERTIFICATE_{suffix}";
        var conversationId = 0;

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();

        var now = DateTime.UtcNow;
        var conversation = new Conversation
        {
            Name = $"Audit cohort {suffix}",
            IsGroup = true,
            CreatedById = ownerTrainerUserId,
            SenderId = ownerTrainerUserId,
            RecieveId = paidStudentUserId
        };
        db.Conversations.Add(conversation);
        await db.SaveChangesAsync();
        conversationId = conversation.Id;

        db.Courses.Add(new Course
        {
            CourseId = courseId,
            TrainerId = ownerTrainerId,
            Title = $"Authorization Audit {suffix}",
            Description = "Security audit fixture course.",
            Type = "Online",
            Category = "Fitness",
            Price = 100,
            StartDate = now.AddDays(-2),
            EndDate = now.AddDays(30),
            Duration = 4,
            MaxParticipants = 10,
            Status = CourseStatus.Published
        });
        db.Payments.Add(new Payment
        {
            PaymentId = paymentId,
            CourseId = courseId,
            StudentId = paidStudentId,
            PaymentAmount = 100,
            PaymentDate = now.AddDays(-1),
            PaymentStatus = PaymentStatus.Paid,
            PaymentType = PaymentType.BankTransfer,
            CreatedAt = now.AddDays(-1),
            UpdatedAt = now.AddDays(-1)
        });
        db.CourseRegistrations.Add(new CourseRegistration
        {
            RegistrationId = $"AUDIT_REGISTRATION_{suffix}",
            CourseId = courseId,
            StudentId = paidStudentId,
            RegistrationType = "FullCourse",
            Mode = "Online",
            Status = "Active",
            PaymentStatus = PaymentStatus.Paid,
            InitialPayment = 100,
            CreatedAt = now.AddDays(-1),
            UpdatedAt = now.AddDays(-1)
        });
        db.GradeCategories.Add(new GradeCategory
        {
            CategoryId = $"AUDIT_GRADE_{suffix}",
            CourseId = courseId,
            Name = "Assessments",
            WeightPercent = 100,
            Order = 1,
            IsDefault = true,
            CreatedAt = now,
            UpdatedAt = now
        });
        db.Lectures.Add(new Lecture
        {
            LectureId = $"AUDIT_LECTURE_{suffix}",
            CourseId = courseId,
            Title = "Audit lesson",
            ActivityType = LearningActivityType.Lesson,
            Order = 1,
            Duration = 10,
            IsPublished = true
        });
        db.CourseAssignments.Add(new CourseAssignment
        {
            AssignmentId = assignmentId,
            CourseId = courseId,
            Title = "Audit assignment",
            Instructions = "Submit a short response.",
            PointsPossible = 100,
            DueAt = now.AddDays(2),
            SubmissionType = AssignmentSubmissionType.Text,
            Status = AssignmentStatus.Published,
            CreatedAt = now,
            UpdatedAt = now
        });
        db.AssignmentSubmissions.Add(new AssignmentSubmission
        {
            SubmissionId = submissionId,
            AssignmentId = assignmentId,
            StudentId = paidStudentId,
            TextResponse = "Private learner submission for audit.",
            Score = 88,
            ScorePercent = 88,
            Status = AssignmentSubmissionStatus.Graded,
            Feedback = "Private trainer feedback.",
            SubmittedAt = now.AddDays(-1),
            GradedAt = now,
            UpdatedAt = now
        });
        db.CourseStudyGroups.Add(new CourseStudyGroup
        {
            StudyGroupId = $"AUDIT_GROUP_{suffix}",
            CourseId = courseId,
            ConversationId = conversationId,
            Name = "Audit cohort",
            Kind = CourseStudyGroupKind.Cohort,
            IsDefaultCohort = true,
            IsActive = true,
            CreatedByUserId = ownerTrainerUserId,
            CreatedAt = now,
            UpdatedAt = now
        });
        db.ConversationParticipants.AddRange(
            new ConversationParticipant
            {
                ConversationId = conversationId,
                UserId = ownerTrainerUserId,
                Role = ParticipantRoles.Owner,
                JoinedAt = now
            },
            new ConversationParticipant
            {
                ConversationId = conversationId,
                UserId = paidStudentUserId,
                Role = ParticipantRoles.Member,
                JoinedAt = now
            });
        db.CourseCertificateSettings.Add(new CourseCertificateSetting
        {
            CourseId = courseId,
            IsEnabled = true,
            TemplateName = "Audit",
            CertificateTitle = "Audit Certificate",
            BodyText = "Audit certificate body.",
            AccentColor = "#1f7a5a",
            RequiredLecturePercent = 0,
            RequirePublishedQuizzes = false,
            RequirePublishedAssignments = false,
            CreatedAt = now,
            UpdatedAt = now
        });
        db.CourseCertificates.Add(new CourseCertificate
        {
            CertificateId = certificateId,
            CourseId = courseId,
            StudentId = paidStudentId,
            VerificationCode = $"AUDIT_VERIFY_{suffix}",
            IssuedAt = now
        });

        await db.SaveChangesAsync();

        return new LmsAuditFixture(
            courseId,
            assignmentId,
            certificateId,
            ownerTrainerEmail,
            otherTrainerEmail,
            paidStudentEmail,
            unpaidStudentEmail,
            ownerTrainerId);
    }

    private sealed record LmsAuditFixture(
        string CourseId,
        string AssignmentId,
        string CertificateId,
        string OwnerTrainerEmail,
        string OtherTrainerEmail,
        string PaidStudentEmail,
        string UnpaidStudentEmail,
        string OwnerTrainerId);
}

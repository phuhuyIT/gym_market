using GymMarket.API.Data;
using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class DemoDataSeederIntegrationTests : BaseIntegrationTests
{
    public DemoDataSeederIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task EnsureSeededAsync_CreatesRepresentativeLmsQaDataset()
    {
        using var scope = Factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IDemoDataSeeder>();
        var db = scope.ServiceProvider.GetRequiredService<GymMarketContext>();

        var first = await seeder.EnsureSeededAsync();
        var second = await seeder.EnsureSeededAsync();

        Assert.True(first.Created);
        Assert.False(second.Created);
        Assert.Equal(DemoDataSeedIds.CourseId, first.CourseId);
        Assert.Equal(DemoDataSeedIds.TrainerEmail, first.TrainerEmail);
        Assert.Equal(DemoDataSeedIds.StudentEmail, first.StudentEmail);

        var course = await db.Courses.SingleAsync(c => c.CourseId == DemoDataSeedIds.CourseId);
        Assert.Equal(CourseStatus.Published, course.Status);
        Assert.Equal(DemoDataSeedIds.TrainerId, course.TrainerId);

        Assert.True(await db.Trainers.AnyAsync(t =>
            t.TrainerId == DemoDataSeedIds.TrainerId &&
            t.ApprovalStatus == TrainerApprovalStatus.Approved));
        Assert.True(await db.Students.AnyAsync(s => s.StudentId == DemoDataSeedIds.StudentId));
        Assert.True(await db.CourseRegistrations.AnyAsync(r =>
            r.CourseId == DemoDataSeedIds.CourseId &&
            r.StudentId == DemoDataSeedIds.StudentId &&
            r.PaymentStatus == PaymentStatus.Paid));
        Assert.True(await db.Payments.AnyAsync(p =>
            p.CourseId == DemoDataSeedIds.CourseId &&
            p.StudentId == DemoDataSeedIds.StudentId &&
            p.PaymentStatus == PaymentStatus.Paid));

        Assert.Equal(2, await db.GradeCategories.CountAsync(c => c.CourseId == DemoDataSeedIds.CourseId));
        Assert.Equal(2, await db.Lectures.CountAsync(l => l.CourseId == DemoDataSeedIds.CourseId));
        Assert.Equal(2, await db.LectureProgresses.CountAsync(p => p.StudentId == DemoDataSeedIds.StudentId));
        Assert.True(await db.CourseAssignments.AnyAsync(a =>
            a.CourseId == DemoDataSeedIds.CourseId &&
            a.Status == AssignmentStatus.Published));
        Assert.True(await db.AssignmentSubmissions.AnyAsync(s =>
            s.StudentId == DemoDataSeedIds.StudentId &&
            s.Status == AssignmentSubmissionStatus.Graded &&
            s.SimilarityScorePercent != null));
        Assert.True(await db.CourseQuizzes.AnyAsync(q =>
            q.CourseId == DemoDataSeedIds.CourseId &&
            q.IsPublished &&
            q.RequireHonorCode &&
            q.TrackProctoringSignals));
        Assert.True(await db.QuizAttempts.AnyAsync(a =>
            a.StudentId == DemoDataSeedIds.StudentId &&
            a.Passed &&
            a.HonorCodeAccepted));
        Assert.True(await db.CourseDiscussionQuestions.AnyAsync(q =>
            q.CourseId == DemoDataSeedIds.CourseId &&
            q.Status == DiscussionQuestionStatus.Answered &&
            q.AcceptedAnswerId != null));
        Assert.True(await db.CourseStudyGroups.AnyAsync(g =>
            g.CourseId == DemoDataSeedIds.CourseId &&
            g.IsDefaultCohort &&
            g.Kind == CourseStudyGroupKind.Cohort));
        Assert.True(await db.CourseAnnouncements.AnyAsync(a =>
            a.CourseId == DemoDataSeedIds.CourseId &&
            a.IsPublished));
        Assert.True(await db.CourseLiveSessions.AnyAsync(s =>
            s.CourseId == DemoDataSeedIds.CourseId &&
            s.Status == CourseLiveSessionStatus.Scheduled));
        Assert.True(await db.CourseCertificateSettings.AnyAsync(s =>
            s.CourseId == DemoDataSeedIds.CourseId &&
            s.IsEnabled &&
            s.RequirePublishedAssignments));
        Assert.True(await db.CourseCertificates.AnyAsync(c =>
            c.CourseId == DemoDataSeedIds.CourseId &&
            c.StudentId == DemoDataSeedIds.StudentId));
        Assert.True(await db.Notifications.CountAsync() >= 3);
    }
}

using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.ClassSchedule;
using GymMarket.API.DTOs.Membership;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GymMarket.API.Tests;

public class ClassScheduleIntegrationTests : BaseIntegrationTests
{
    public ClassScheduleIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task BookSession_WithoutActiveMembership_ReturnsForbidden()
    {
        var session = await CreateSession("Open Strength", capacity: 8);
        await AuthenticateAsync(email: "class-no-membership@example.com", role: "Student");

        var response = await Client.PostAsync($"/api/ClassSchedule/sessions/{session.ClassSessionId}/book", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task BookSession_WithActiveMembership_ReturnsBooking()
    {
        var plan = await CreatePlan("Class Access", 30, 59);
        var session = await CreateSession("Mobility Lab", capacity: 6);
        await AuthenticateAsync(email: "class-book-student@example.com", role: "Student");
        await Subscribe(plan.PlanId);

        var response = await Client.PostAsync($"/api/ClassSchedule/sessions/{session.ClassSessionId}/book", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var booking = await response.Content.ReadFromJsonAsync<ClassBookingDto>();
        Assert.NotNull(booking);
        Assert.Equal(session.ClassSessionId, booking!.ClassSessionId);
        Assert.Equal("Booked", booking.Status);

        var sessions = await Client.GetFromJsonAsync<List<ClassSessionDto>>("/api/ClassSchedule/sessions");
        Assert.Contains(sessions!, s => s.ClassSessionId == session.ClassSessionId && s.IsBooked);
    }

    [Fact]
    public async Task BookSession_DuplicateOrFull_ReturnsConflict()
    {
        var plan = await CreatePlan("Capacity Plan", 30, 49);
        var session = await CreateSession("Small Group Lift", capacity: 1);

        await AuthenticateAsync(email: "class-capacity-first@example.com", role: "Student");
        await Subscribe(plan.PlanId);
        var firstBooking = await Client.PostAsync($"/api/ClassSchedule/sessions/{session.ClassSessionId}/book", null);
        firstBooking.EnsureSuccessStatusCode();

        var duplicate = await Client.PostAsync($"/api/ClassSchedule/sessions/{session.ClassSessionId}/book", null);
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        await AuthenticateAsync(email: "class-capacity-second@example.com", role: "Student");
        await Subscribe(plan.PlanId);
        var full = await Client.PostAsync($"/api/ClassSchedule/sessions/{session.ClassSessionId}/book", null);
        Assert.Equal(HttpStatusCode.Conflict, full.StatusCode);
    }

    [Fact]
    public async Task CancelBooking_FreesCapacityForAnotherStudent()
    {
        var plan = await CreatePlan("Cancel Frees Capacity", 30, 49);
        var session = await CreateSession("Cycle Row", capacity: 1);

        await AuthenticateAsync(email: "class-cancel-first@example.com", role: "Student");
        await Subscribe(plan.PlanId);
        var firstBookingResponse = await Client.PostAsync($"/api/ClassSchedule/sessions/{session.ClassSessionId}/book", null);
        firstBookingResponse.EnsureSuccessStatusCode();
        var firstBooking = (await firstBookingResponse.Content.ReadFromJsonAsync<ClassBookingDto>())!;

        var cancel = await Client.PostAsync($"/api/ClassSchedule/bookings/{firstBooking.BookingId}/cancel", null);
        Assert.Equal(HttpStatusCode.OK, cancel.StatusCode);

        await AuthenticateAsync(email: "class-cancel-second@example.com", role: "Student");
        await Subscribe(plan.PlanId);
        var secondBooking = await Client.PostAsync($"/api/ClassSchedule/sessions/{session.ClassSessionId}/book", null);

        Assert.Equal(HttpStatusCode.OK, secondBooking.StatusCode);
    }

    [Fact]
    public async Task MarkAttendance_AsAdmin_UpdatesBookingStatus()
    {
        var plan = await CreatePlan("Attendance Plan", 30, 49);
        var session = await CreateSession("HIIT Check-in", capacity: 3);

        await AuthenticateAsync(email: "class-attendance-student@example.com", role: "Student");
        await Subscribe(plan.PlanId);
        var bookingResponse = await Client.PostAsync($"/api/ClassSchedule/sessions/{session.ClassSessionId}/book", null);
        bookingResponse.EnsureSuccessStatusCode();
        var booking = (await bookingResponse.Content.ReadFromJsonAsync<ClassBookingDto>())!;

        await AuthenticateAsAdminAsync(email: "class-attendance-admin@example.com");
        var response = await Client.PostAsJsonAsync($"/api/ClassSchedule/bookings/{booking.BookingId}/attendance", new MarkClassAttendanceDto
        {
            Status = "Attended"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var marked = await response.Content.ReadFromJsonAsync<ClassBookingDto>();
        Assert.Equal("Attended", marked!.Status);
        Assert.NotNull(marked.AttendanceMarkedAt);
    }

    private async Task<ClassSessionDto> CreateSession(string title, int capacity)
    {
        await AuthenticateAsAdminAsync(email: $"class-admin-{Guid.NewGuid():N}@example.com");
        var response = await Client.PostAsJsonAsync("/api/ClassSchedule/sessions", new UpsertClassSessionDto
        {
            Title = title,
            Description = $"{title} test session",
            StartsAt = DateTime.UtcNow.AddDays(2),
            EndsAt = DateTime.UtcNow.AddDays(2).AddHours(1),
            Capacity = capacity,
            Location = "Studio A",
            Status = "Scheduled"
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClassSessionDto>())!;
    }

    private async Task<MembershipPlanDto> CreatePlan(string name, int durationDays, decimal price)
    {
        await AuthenticateAsAdminAsync(email: $"class-plan-admin-{Guid.NewGuid():N}@example.com");
        var response = await Client.PostAsJsonAsync("/api/Memberships/plans", new UpsertMembershipPlanDto
        {
            Name = name,
            Description = $"{durationDays}-day class access",
            DurationDays = durationDays,
            Price = price,
            IsActive = true
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<MembershipPlanDto>())!;
    }

    private async Task Subscribe(string planId)
    {
        var response = await Client.PostAsJsonAsync("/api/Memberships/subscribe", new SubscribeMembershipDto
        {
            PlanId = planId
        });

        response.EnsureSuccessStatusCode();
    }
}

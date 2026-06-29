using System.Net;
using System.Net.Http.Json;
using GymMarket.API.DTOs.Admin;
using GymMarket.API.DTOs.Notifications;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class NotificationsIntegrationTests : BaseIntegrationTests
{
    public NotificationsIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task MarkTypeAsRead_OnlyClearsNotificationsOfSelectedType()
    {
        const string email = "notification-type-student@example.com";
        await AuthenticateAsync(email: email, role: "Student");
        await SeedNotification(email, NotificationTypes.Class, "Class reminder", "/client/classes");
        await SeedNotification(email, NotificationTypes.Class, "Class cancelled", "/client/classes");
        await SeedNotification(email, NotificationTypes.Workout, "Workout waiting", "/client/workouts");

        var response = await Client.PostAsync($"/api/Notifications/mark-type-read/{NotificationTypes.Class}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.NotNull(notifications);
        Assert.All(notifications!.Where(n => n.Type == NotificationTypes.Class), n => Assert.True(n.IsRead));
        Assert.Contains(notifications, n => n.Type == NotificationTypes.Workout && !n.IsRead);
    }

    [Fact]
    public async Task GetNotifications_CanFilterByTypeAndReadStatus()
    {
        const string email = "notification-filter-student@example.com";
        await AuthenticateAsync(email: email, role: "Student");
        await SeedNotification(email, NotificationTypes.Class, "Class reminder", "/client/classes");
        await SeedNotification(email, NotificationTypes.Class, "Class cancelled", "/client/classes");
        await SeedNotification(email, NotificationTypes.Workout, "Workout waiting", "/client/workouts");
        await Client.PostAsync($"/api/Notifications/mark-type-read/{NotificationTypes.Class}", null);

        var classRead = await Client.GetFromJsonAsync<List<NotificationDto>>(
            $"/api/Notifications/get-notifications?type={NotificationTypes.Class}&isRead=true&take=10");
        var unread = await Client.GetFromJsonAsync<List<NotificationDto>>(
            "/api/Notifications/get-notifications?isRead=false&take=10");

        Assert.NotNull(classRead);
        Assert.Equal(2, classRead!.Count);
        Assert.All(classRead, n =>
        {
            Assert.Equal(NotificationTypes.Class, n.Type);
            Assert.True(n.IsRead);
        });
        Assert.NotNull(unread);
        Assert.Single(unread!);
        Assert.Equal(NotificationTypes.Workout, unread![0].Type);
        Assert.False(unread[0].IsRead);
    }

    [Fact]
    public async Task GetPreferences_ReturnsDefaultEnabledCategories()
    {
        const string email = "notification-default-preferences@example.com";
        await AuthenticateAsync(email: email, role: "Student");

        var preferences = await Client.GetFromJsonAsync<List<NotificationPreferenceDto>>("/api/Notifications/preferences");

        Assert.NotNull(preferences);
        Assert.Equal(NotificationTypes.All.Count, preferences!.Count);
        Assert.All(preferences, preference => Assert.True(preference.InAppEnabled));
        Assert.All(preferences, preference => Assert.True(preference.EmailEnabled));
        Assert.Contains(preferences, preference => preference.Type == NotificationTypes.Class && preference.Label == "Class");
    }

    [Fact]
    public async Task DisabledPreferencePreventsNewNotificationsOfThatType()
    {
        const string email = "notification-disabled-preference@example.com";
        await AuthenticateAsync(email: email, role: "Student");

        var updateResponse = await Client.PutAsJsonAsync("/api/Notifications/preferences", new UpdateNotificationPreferencesDto
        {
            Preferences =
            [
                new NotificationPreferenceUpdateDto
                {
                    Type = NotificationTypes.Class,
                    InAppEnabled = false,
                    EmailEnabled = true,
                },
            ],
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        await SeedNotification(email, NotificationTypes.Class, "Class reminder", "/client/classes");
        await SeedNotification(email, NotificationTypes.Workout, "Workout waiting", "/client/workouts");

        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");

        Assert.NotNull(notifications);
        Assert.DoesNotContain(notifications!, n => n.Type == NotificationTypes.Class);
        Assert.Contains(notifications, n => n.Type == NotificationTypes.Workout);
    }

    [Fact]
    public async Task EnabledEmailPreferenceSendsNotificationEmail()
    {
        const string email = "notification-email-enabled@example.com";
        await AuthenticateAsync(email: email, role: "Student");
        EmailSender.Clear();

        await SeedNotification(email, NotificationTypes.Workout, "Workout waiting", "/client/workouts");

        var sentEmail = Assert.Single(EmailSender.SentEmails);
        Assert.Equal(email, sentEmail.ToEmail);
        Assert.Equal("Workout waiting", sentEmail.Subject);
        Assert.Contains("Workout waiting content", sentEmail.HtmlBody);
        Assert.Contains("http://localhost:4200/client/workouts", sentEmail.HtmlBody);
    }

    [Fact]
    public async Task DisabledEmailPreferenceSuppressesEmailButKeepsInAppNotification()
    {
        const string email = "notification-email-disabled@example.com";
        await AuthenticateAsync(email: email, role: "Student");

        var updateResponse = await Client.PutAsJsonAsync("/api/Notifications/preferences", new UpdateNotificationPreferencesDto
        {
            Preferences =
            [
                new NotificationPreferenceUpdateDto
                {
                    Type = NotificationTypes.Membership,
                    InAppEnabled = true,
                    EmailEnabled = false,
                },
            ],
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        EmailSender.Clear();

        await SeedNotification(email, NotificationTypes.Membership, "Membership expiring", "/client/memberships");

        Assert.Empty(EmailSender.SentEmails);
        var notifications = await Client.GetFromJsonAsync<List<NotificationDto>>("/api/Notifications/get-notifications");
        Assert.NotNull(notifications);
        Assert.Contains(notifications!, n => n.Type == NotificationTypes.Membership && n.Title == "Membership expiring");
    }

    [Fact]
    public async Task AdminTemplateIsUsedForNotificationEmail()
    {
        const string adminEmail = "notification-template-admin@example.com";
        const string studentEmail = "notification-template-student@example.com";
        await AuthenticateAsAdminAsync(email: adminEmail);

        var updateResponse = await Client.PutAsJsonAsync(
            $"/api/Admin/notifications/templates/{NotificationTypes.Workout}",
            new UpdateNotificationTemplateDto
            {
                SubjectTemplate = "Workout update for {{userName}}",
                BodyTemplate = "<p>{{category}}</p><p>{{content}}</p><p>{{actionUrl}}</p>",
                IsActive = true,
            });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var template = await updateResponse.Content.ReadFromJsonAsync<AdminNotificationTemplateDto>();
        Assert.NotNull(template);
        Assert.Equal(NotificationTypes.Workout, template!.Type);

        await AuthenticateAsync(email: studentEmail, role: "Student");
        EmailSender.Clear();

        await SeedNotification(studentEmail, NotificationTypes.Workout, "Workout waiting", "/client/workouts");

        var sentEmail = Assert.Single(EmailSender.SentEmails);
        Assert.Equal("Workout update for Test User", sentEmail.Subject);
        Assert.Contains("<p>Workout</p>", sentEmail.HtmlBody);
        Assert.Contains("<p>Workout waiting content</p>", sentEmail.HtmlBody);
        Assert.Contains("<p>http://localhost:4200/client/workouts</p>", sentEmail.HtmlBody);
    }

    [Fact]
    public async Task AdminDeliveryHistoryIncludesSentAndSkippedOutcomes()
    {
        const string adminEmail = "notification-delivery-admin@example.com";
        const string studentEmail = "notification-delivery-student@example.com";
        await AuthenticateAsync(email: studentEmail, role: "Student");

        var updateResponse = await Client.PutAsJsonAsync("/api/Notifications/preferences", new UpdateNotificationPreferencesDto
        {
            Preferences =
            [
                new NotificationPreferenceUpdateDto
                {
                    Type = NotificationTypes.Class,
                    InAppEnabled = false,
                    EmailEnabled = false,
                },
            ],
        });
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        await SeedNotification(studentEmail, NotificationTypes.Class, "Class reminder", "/client/classes");
        await SeedNotification(studentEmail, NotificationTypes.Workout, "Workout waiting", "/client/workouts");

        await AuthenticateAsAdminAsync(email: adminEmail);

        var classDeliveries = await Client.GetFromJsonAsync<PagedResult<NotificationDeliveryLogDto>>(
            $"/api/Admin/notifications/deliveries?type={NotificationTypes.Class}&pageSize=20");
        var sentDeliveries = await Client.GetFromJsonAsync<PagedResult<NotificationDeliveryLogDto>>(
            "/api/Admin/notifications/deliveries?status=sent&pageSize=20");

        Assert.NotNull(classDeliveries);
        Assert.Contains(classDeliveries!.Items, log =>
            log.Type == NotificationTypes.Class &&
            log.Channel == NotificationDeliveryChannels.InApp &&
            log.Status == NotificationDeliveryStatuses.Skipped);
        Assert.Contains(classDeliveries.Items, log =>
            log.Type == NotificationTypes.Class &&
            log.Channel == NotificationDeliveryChannels.Email &&
            log.Status == NotificationDeliveryStatuses.Skipped);

        Assert.NotNull(sentDeliveries);
        Assert.Contains(sentDeliveries!.Items, log =>
            log.Type == NotificationTypes.Workout &&
            log.Channel == NotificationDeliveryChannels.InApp &&
            log.Status == NotificationDeliveryStatuses.Sent);
        Assert.Contains(sentDeliveries.Items, log =>
            log.Type == NotificationTypes.Workout &&
            log.Channel == NotificationDeliveryChannels.Email &&
            log.Status == NotificationDeliveryStatuses.Sent);
    }

    private async Task SeedNotification(string email, string type, string title, string link)
    {
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var user = await userManager.FindByEmailAsync(email);

        await notificationRepository.NotifyUser(
            user!.Id,
            type,
            title,
            $"{title} content",
            link);
    }
}

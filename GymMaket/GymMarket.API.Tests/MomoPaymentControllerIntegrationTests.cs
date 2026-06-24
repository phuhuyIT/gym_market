using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GymMarket.API.Data;
using GymMarket.API.DTOs.Momo;
using GymMarket.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymMarket.API.Tests;

public class MomoPaymentControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string AccessKey = "test-access-key";
    private const string SecretKey = "test-secret-key";
    private const string PartnerCode = "MOMO";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public MomoPaymentControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var dbName = Guid.NewGuid().ToString();
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["MomoAPI:AccessKey"] = AccessKey,
                    ["MomoAPI:SecretKey"] = SecretKey,
                    ["MomoAPI:PartnerCode"] = PartnerCode,
                    ["MomoAPI:PaymentRedirectUrl"] = "http://localhost:4200/client/course-registration"
                });
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GymMarketContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<GymMarketContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
            });
        });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task PaymentCallBack_ValidSuccess_RedirectsToCoursePaymentAndMarksPaid()
    {
        const string courseId = "MOMO_CALLBACK_SUCCESS_COURSE";
        const string studentId = "MOMO_CALLBACK_SUCCESS_STUDENT";
        const string paymentId = "MOMO_CALLBACK_SUCCESS_ORDER";
        await SeedMomoPaymentAsync(courseId, studentId, paymentId);

        var callback = CreateCallback(paymentId, courseId, studentId, Defaults.MomoSuccessResultCode, "Successful.");
        var response = await _client.GetAsync("/api/MomoPayment/PaymentCallBack?" + ToQueryString(callback));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var location = response.Headers.Location!.ToString();
        Assert.StartsWith($"http://localhost:4200/client/course-payment/{courseId}", location);
        Assert.Contains("momoResult=success", location);
        Assert.Contains("momoCode=0", location);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payment = await context.Payments.SingleAsync(p => p.PaymentId == paymentId);
        var registration = await context.CourseRegistrations.SingleAsync(r => r.CourseId == courseId);
        Assert.Equal(PaymentStatus.Paid, payment.PaymentStatus);
        Assert.Equal(PaymentStatus.Paid, registration.PaymentStatus);
    }

    [Fact]
    public async Task MomoNotify_FailedCallback_CancelsPaymentAndRegistration()
    {
        const string courseId = "MOMO_NOTIFY_CANCEL_COURSE";
        const string studentId = "MOMO_NOTIFY_CANCEL_STUDENT";
        const string paymentId = "MOMO_NOTIFY_CANCEL_ORDER";
        await SeedMomoPaymentAsync(courseId, studentId, paymentId);

        var callback = CreateCallback(paymentId, courseId, studentId, 1006, "User canceled payment.");
        var response = await _client.PostAsJsonAsync("/api/MomoPayment/MomoNotify", callback);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("canceled", json.RootElement.GetProperty("message").GetString());
        Assert.Equal(courseId, json.RootElement.GetProperty("courseId").GetString());

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payment = await context.Payments.SingleAsync(p => p.PaymentId == paymentId);
        var registration = await context.CourseRegistrations.SingleAsync(r => r.CourseId == courseId);
        Assert.Equal(PaymentStatus.Canceled, payment.PaymentStatus);
        Assert.Equal("User canceled payment.", payment.Note);
        Assert.Equal(PaymentStatus.Canceled, registration.PaymentStatus);
    }

    [Fact]
    public async Task MomoNotify_InvalidSignature_ReturnsBadRequestAndLeavesPaymentPending()
    {
        const string courseId = "MOMO_NOTIFY_BAD_SIGNATURE_COURSE";
        const string studentId = "MOMO_NOTIFY_BAD_SIGNATURE_STUDENT";
        const string paymentId = "MOMO_NOTIFY_BAD_SIGNATURE_ORDER";
        await SeedMomoPaymentAsync(courseId, studentId, paymentId);

        var callback = CreateCallback(paymentId, courseId, studentId, Defaults.MomoSuccessResultCode, "Successful.");
        callback.Signature = "invalid-signature";
        var response = await _client.PostAsJsonAsync("/api/MomoPayment/MomoNotify", callback);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payment = await context.Payments.SingleAsync(p => p.PaymentId == paymentId);
        var registration = await context.CourseRegistrations.SingleAsync(r => r.CourseId == courseId);
        Assert.Equal(PaymentStatus.Pending, payment.PaymentStatus);
        Assert.Equal(PaymentStatus.Pending, registration.PaymentStatus);
    }

    [Fact]
    public async Task MomoNotify_DuplicateSuccessCallback_IsIdempotent()
    {
        const string courseId = "MOMO_NOTIFY_DUPLICATE_COURSE";
        const string studentId = "MOMO_NOTIFY_DUPLICATE_STUDENT";
        const string paymentId = "MOMO_NOTIFY_DUPLICATE_ORDER";
        await SeedMomoPaymentAsync(courseId, studentId, paymentId, includePendingBankTransfer: true);

        var callback = CreateCallback(paymentId, courseId, studentId, Defaults.MomoSuccessResultCode, "Successful.");
        var first = await _client.PostAsJsonAsync("/api/MomoPayment/MomoNotify", callback);
        var second = await _client.PostAsJsonAsync("/api/MomoPayment/MomoNotify", callback);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        var payments = await context.Payments.Where(p => p.CourseId == courseId).ToListAsync();
        var momoPayment = payments.Single(p => p.PaymentId == paymentId);
        var bankPayment = payments.Single(p => p.PaymentType == PaymentType.BankTransfer);
        var registration = await context.CourseRegistrations.SingleAsync(r => r.CourseId == courseId);

        Assert.Equal(2, payments.Count);
        Assert.Equal(PaymentStatus.Paid, momoPayment.PaymentStatus);
        Assert.Equal(PaymentStatus.Canceled, bankPayment.PaymentStatus);
        Assert.Equal(PaymentStatus.Paid, registration.PaymentStatus);
    }

    private async Task SeedMomoPaymentAsync(
        string courseId,
        string studentId,
        string paymentId,
        bool includePendingBankTransfer = false)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GymMarketContext>();
        context.Courses.Add(new Course
        {
            CourseId = courseId,
            Title = "Momo Callback Course",
            Price = 100,
            Status = CourseStatus.Published,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        });
        context.CourseRegistrations.Add(new CourseRegistration
        {
            RegistrationId = Guid.NewGuid().ToString(),
            CourseId = courseId,
            StudentId = studentId,
            Status = PaymentStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        context.Payments.Add(new Payment
        {
            PaymentId = paymentId,
            CourseId = courseId,
            StudentId = studentId,
            PaymentAmount = 100,
            PaymentStatus = PaymentStatus.Pending,
            PaymentType = PaymentType.Momo,
            Note = "MOMO-TEST",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        if (includePendingBankTransfer)
        {
            context.Payments.Add(new Payment
            {
                PaymentId = Guid.NewGuid().ToString(),
                CourseId = courseId,
                StudentId = studentId,
                PaymentAmount = 100,
                PaymentStatus = PaymentStatus.Pending,
                PaymentType = PaymentType.BankTransfer,
                Note = "GYM-TEST",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }

    private static MomoCallbackDto CreateCallback(
        string paymentId,
        string courseId,
        string studentId,
        int resultCode,
        string message)
    {
        var extraData = Convert.ToBase64String(Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(new { StudentId = studentId, CourseId = courseId })));

        var callback = new MomoCallbackDto
        {
            PartnerCode = PartnerCode,
            OrderId = paymentId,
            RequestId = paymentId,
            Amount = "100",
            OrderInfo = "Payment for course",
            OrderType = "momo_wallet",
            TransId = "123456789",
            ResultCode = resultCode,
            Message = message,
            PayType = "qr",
            ResponseTime = "1719000000000",
            ExtraData = extraData
        };
        callback.Signature = Sign(callback);
        return callback;
    }

    private static string ToQueryString(MomoCallbackDto callback)
    {
        var values = new Dictionary<string, string?>
        {
            ["partnerCode"] = callback.PartnerCode,
            ["orderId"] = callback.OrderId,
            ["requestId"] = callback.RequestId,
            ["amount"] = callback.Amount,
            ["orderInfo"] = callback.OrderInfo,
            ["orderType"] = callback.OrderType,
            ["transId"] = callback.TransId,
            ["resultCode"] = callback.ResultCode.ToString(),
            ["message"] = callback.Message,
            ["payType"] = callback.PayType,
            ["responseTime"] = callback.ResponseTime,
            ["extraData"] = callback.ExtraData,
            ["signature"] = callback.Signature
        };

        return string.Join("&", values.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}"));
    }

    private static string Sign(MomoCallbackDto callback)
    {
        var rawData =
            $"accessKey={AccessKey}" +
            $"&amount={callback.Amount}" +
            $"&extraData={callback.ExtraData}" +
            $"&message={callback.Message}" +
            $"&orderId={callback.OrderId}" +
            $"&orderInfo={callback.OrderInfo}" +
            $"&orderType={callback.OrderType}" +
            $"&partnerCode={callback.PartnerCode}" +
            $"&payType={callback.PayType}" +
            $"&requestId={callback.RequestId}" +
            $"&responseTime={callback.ResponseTime}" +
            $"&resultCode={callback.ResultCode}" +
            $"&transId={callback.TransId}";

        var keyBytes = Encoding.UTF8.GetBytes(SecretKey);
        var messageBytes = Encoding.UTF8.GetBytes(rawData);
        using var hmac = new HMACSHA256(keyBytes);
        return BitConverter.ToString(hmac.ComputeHash(messageBytes)).Replace("-", "").ToLowerInvariant();
    }
}

namespace GymMarket.API.Services;

public class NotificationDigestHostedService : BackgroundService
{
    private static readonly TimeSpan SweepInterval = TimeSpan.FromHours(6);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationDigestHostedService> _logger;

    public NotificationDigestHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationDigestHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunSweepAsync(stoppingToken);

        using var timer = new PeriodicTimer(SweepInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunSweepAsync(stoppingToken);
        }
    }

    private async Task RunSweepAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var digestService = scope.ServiceProvider.GetRequiredService<INotificationDigestService>();
            var sentCount = await digestService.SendDueDigestsAsync(cancellationToken: stoppingToken);

            if (sentCount > 0)
            {
                _logger.LogInformation("Sent {DigestCount} notification digest emails.", sentCount);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification digest emails.");
        }
    }
}

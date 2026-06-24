namespace GymMarket.API.Services
{
    public class RegistrationExpiryHostedService : BackgroundService
    {
        private static readonly TimeSpan SweepInterval = TimeSpan.FromMinutes(2);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RegistrationExpiryHostedService> _logger;

        public RegistrationExpiryHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<RegistrationExpiryHostedService> logger)
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
                var expiryService = scope.ServiceProvider.GetRequiredService<IRegistrationExpiryService>();
                var expiredCount = await expiryService.ExpireStalePendingRegistrationsAsync(
                    cancellationToken: stoppingToken);

                if (expiredCount > 0)
                {
                    _logger.LogInformation("Expired {ExpiredCount} stale course registrations.", expiredCount);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to expire stale course registrations.");
            }
        }
    }
}

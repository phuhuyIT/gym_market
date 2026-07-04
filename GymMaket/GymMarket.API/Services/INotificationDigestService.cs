namespace GymMarket.API.Services;

public interface INotificationDigestService
{
    Task<int> SendDueDigestsAsync(DateTime? utcNow = null, CancellationToken cancellationToken = default);
}

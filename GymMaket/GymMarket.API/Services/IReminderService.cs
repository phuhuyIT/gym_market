namespace GymMarket.API.Services;

public interface IReminderService
{
    Task<int> SendDueRemindersAsync(CancellationToken cancellationToken = default);
}

namespace GymMarket.API.Services
{
    public interface IRegistrationExpiryService
    {
        Task<int> ExpireStalePendingRegistrationsAsync(
            string? courseId = null,
            string? studentId = null,
            CancellationToken cancellationToken = default);
    }
}

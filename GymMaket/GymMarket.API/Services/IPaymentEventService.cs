namespace GymMarket.API.Services
{
    public interface IPaymentEventService
    {
        Task AddPaymentEventAsync(
            string paymentId,
            string eventType,
            string? oldStatus,
            string? newStatus,
            string source,
            string? message = null,
            string? rawPayload = null,
            CancellationToken cancellationToken = default);
    }
}

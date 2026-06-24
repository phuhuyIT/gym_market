using GymMarket.API.Data;
using GymMarket.API.Models;

namespace GymMarket.API.Services
{
    public class PaymentEventService : IPaymentEventService
    {
        private readonly GymMarketContext _context;

        public PaymentEventService(GymMarketContext context)
        {
            _context = context;
        }

        public Task AddPaymentEventAsync(
            string paymentId,
            string eventType,
            string? oldStatus,
            string? newStatus,
            string source,
            string? message = null,
            string? rawPayload = null,
            CancellationToken cancellationToken = default)
        {
            _context.PaymentEvents.Add(new PaymentEvent
            {
                PaymentEventId = Guid.NewGuid().ToString(),
                PaymentId = paymentId,
                EventType = eventType,
                OldStatus = PaymentStatus.Normalize(oldStatus),
                NewStatus = PaymentStatus.Normalize(newStatus),
                Source = source,
                Message = message,
                RawPayload = rawPayload,
                CreatedAt = DateTime.UtcNow
            });

            return Task.CompletedTask;
        }
    }
}

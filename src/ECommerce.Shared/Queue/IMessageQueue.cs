using ECommerce.Shared.DTOs;

namespace ECommerce.Shared.Queue;

public interface IMessageQueue
{
    Task PublishAsync(PaymentMessage message);
    IAsyncEnumerable<PaymentMessage> ConsumeAsync(CancellationToken cancellationToken);
}

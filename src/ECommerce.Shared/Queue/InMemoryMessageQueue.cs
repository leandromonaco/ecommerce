using ECommerce.Shared.DTOs;
using System.Threading.Channels;

namespace ECommerce.Shared.Queue;

public class InMemoryMessageQueue : IMessageQueue
{
    private readonly Channel<PaymentMessage> _channel = Channel.CreateUnbounded<PaymentMessage>();

    public async Task PublishAsync(PaymentMessage message)
    {
        await _channel.Writer.WriteAsync(message);
    }

    public async IAsyncEnumerable<PaymentMessage> ConsumeAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return message;
        }
    }
}

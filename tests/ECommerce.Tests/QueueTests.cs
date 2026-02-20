using ECommerce.Shared.DTOs;
using ECommerce.Shared.Queue;

namespace ECommerce.Tests;

public class QueueTests
{
    [Fact]
    public async Task InMemoryMessageQueue_ShouldPublishAndConsume()
    {
        var queue = new InMemoryMessageQueue();
        var message = new PaymentMessage(OrderId: 1, Amount: 99.99m, PaymentId: Guid.NewGuid());

        await queue.PublishAsync(message);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        PaymentMessage? received = null;

        await foreach (var msg in queue.ConsumeAsync(cts.Token))
        {
            received = msg;
            break;
        }

        Assert.NotNull(received);
        Assert.Equal(message.OrderId, received.OrderId);
        Assert.Equal(message.Amount, received.Amount);
        Assert.Equal(message.PaymentId, received.PaymentId);
    }

    [Fact]
    public async Task InMemoryMessageQueue_ShouldConsumeMultipleMessages()
    {
        var queue = new InMemoryMessageQueue();
        var messages = new[]
        {
            new PaymentMessage(1, 10m, Guid.NewGuid()),
            new PaymentMessage(2, 20m, Guid.NewGuid()),
            new PaymentMessage(3, 30m, Guid.NewGuid())
        };

        foreach (var msg in messages)
            await queue.PublishAsync(msg);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var received = new List<PaymentMessage>();

        await foreach (var msg in queue.ConsumeAsync(cts.Token))
        {
            received.Add(msg);
            if (received.Count == 3) break;
        }

        Assert.Equal(3, received.Count);
        Assert.Equal(1, received[0].OrderId);
        Assert.Equal(2, received[1].OrderId);
        Assert.Equal(3, received[2].OrderId);
    }
}

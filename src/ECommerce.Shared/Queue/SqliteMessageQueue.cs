using ECommerce.Shared.Data;
using ECommerce.Shared.DTOs;
using ECommerce.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace ECommerce.Shared.Queue;

public class SqliteMessageQueue : IMessageQueue
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SqliteMessageQueue> _logger;

    public SqliteMessageQueue(IServiceScopeFactory scopeFactory, ILogger<SqliteMessageQueue> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task PublishAsync(PaymentMessage message)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.QueueMessages.Add(new QueueMessage
        {
            OrderId = message.OrderId,
            Amount = message.Amount,
            PaymentId = message.PaymentId
        });
        await db.SaveChangesAsync();
    }

    public async IAsyncEnumerable<PaymentMessage> ConsumeAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            List<PaymentMessage> toProcess = new();

            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var pending = await db.QueueMessages
                    .Where(m => m.ProcessedAt == null)
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync(cancellationToken);

                foreach (var qm in pending)
                {
                    qm.ProcessedAt = DateTime.UtcNow;
                    toProcess.Add(new PaymentMessage(qm.OrderId, qm.Amount, qm.PaymentId));
                }

                if (toProcess.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Found {toProcess.Count} pending message(s) in queue");
                    Console.ResetColor();
                    await db.SaveChangesAsync(cancellationToken);
                }
            }

            foreach (var message in toProcess)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Dequeuing message — OrderId: {message.OrderId}, PaymentId: {message.PaymentId}, Amount: {message.Amount:C}");
                Console.ResetColor();
                yield return message;
            }

            if (toProcess.Count == 0)
            {
                _logger.LogDebug("No pending messages. Polling again in 5 seconds");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}

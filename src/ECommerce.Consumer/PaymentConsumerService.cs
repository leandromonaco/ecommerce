using ECommerce.Shared.Data;
using ECommerce.Shared.Models;
using ECommerce.Shared.Queue;

namespace ECommerce.Consumer;

public class PaymentConsumerService : BackgroundService
{
    private readonly IMessageQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentConsumerService> _logger;

    public PaymentConsumerService(
        IMessageQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentConsumerService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment consumer started");

        await foreach (var message in _queue.ConsumeAsync(stoppingToken))
        {
            _logger.LogInformation("Processing payment for Order {OrderId}, Amount: {Amount}",
                message.OrderId, message.Amount);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var order = await db.Orders.FindAsync(new object[] { message.OrderId }, stoppingToken);
            if (order is null)
            {
                _logger.LogWarning("Order {OrderId} not found", message.OrderId);
                continue;
            }

            try
            {
                order.Status = OrderStatus.Paid;
                order.PaymentId = message.PaymentId;
                await db.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Payment for Order {OrderId} processed successfully", message.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process payment for Order {OrderId}", message.OrderId);
                order.Status = OrderStatus.Failed;
                await db.SaveChangesAsync(stoppingToken);
            }
        }
    }
}

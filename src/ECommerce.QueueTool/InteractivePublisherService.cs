using ECommerce.Shared.DTOs;
using ECommerce.Shared.Queue;

namespace ECommerce.QueueTool;

public class InteractivePublisherService
{
    private readonly IMessageQueue _queue;
    private readonly ILogger<InteractivePublisherService> _logger;

    public InteractivePublisherService(
        IMessageQueue queue,
        ILogger<InteractivePublisherService> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("=== Queue Publisher Tool ===");
        Console.WriteLine("Commands:");
        Console.WriteLine("  <orderId> <amount>  — publish a payment message");
        Console.WriteLine("  exit               — quit");
        Console.WriteLine();

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();

            if (line is null || line.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2
                || !int.TryParse(parts[0], out var orderId)
                || !decimal.TryParse(parts[1], out var amount))
            {
                Console.WriteLine("Invalid input. Usage: <orderId> <amount>");
                continue;
            }

            var message = new PaymentMessage(orderId, amount, Guid.NewGuid());
            await _queue.PublishAsync(message);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Published: OrderId={orderId}, Amount={amount:C}, PaymentId={message.PaymentId}");
            Console.ResetColor();
        }
    }
}

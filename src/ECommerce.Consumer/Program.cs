using ECommerce.Shared.Data;
using ECommerce.Shared.Queue;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Consumer;

/// <summary>
/// Standalone entry point for running the payment consumer as an independent worker service.
/// When deployed alongside the API, the PaymentConsumerService is registered as a hosted service
/// in the API's Program.cs and shares the same in-memory queue instance.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureLogging(logging =>
        {
            logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
        });

        builder.ConfigureServices(services =>
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(DatabaseConfiguration.ConnectionString));

            services.AddSingleton<SqliteMessageQueue>();
            services.AddSingleton<IMessageQueue>(sp => sp.GetRequiredService<SqliteMessageQueue>());

            services.AddHostedService<PaymentConsumerService>();
        });

        var host = builder.Build();

        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        host.Run();
    }
}

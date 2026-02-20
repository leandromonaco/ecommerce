using ECommerce.Consumer;
using ECommerce.QueueTool;
using ECommerce.Shared.Data;
using ECommerce.Shared.Queue;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSimpleConsole(options =>
        {
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        });
        logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
    })
    .ConfigureServices(services =>
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(DatabaseConfiguration.ConnectionString));

        services.AddSingleton<SqliteMessageQueue>();
        services.AddSingleton<IMessageQueue>(sp => sp.GetRequiredService<SqliteMessageQueue>());

        services.AddHostedService<PaymentConsumerService>();
        services.AddSingleton<InteractivePublisherService>();
    });

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

await host.StartAsync();

var publisher = host.Services.GetRequiredService<InteractivePublisherService>();
await publisher.RunAsync();

await host.StopAsync();

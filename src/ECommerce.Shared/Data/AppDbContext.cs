using ECommerce.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Shared.Data;

public class AppDbContext : DbContext
{
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<QueueMessage> QueueMessages => Set<QueueMessage>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(DatabaseConfiguration.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<QueueMessage>().ToTable("Queue");
    }
}

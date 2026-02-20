namespace ECommerce.Shared.Models;

public class Order
{
    public int Id { get; set; }
    public string CartId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? PaymentId { get; set; }
}

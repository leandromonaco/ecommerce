namespace ECommerce.Shared.DTOs;

public record OrderResponse(int OrderId, string Status, decimal TotalAmount, DateTime CreatedAt);

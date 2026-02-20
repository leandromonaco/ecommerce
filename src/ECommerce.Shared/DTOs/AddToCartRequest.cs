namespace ECommerce.Shared.DTOs;

public record AddToCartRequest(string ProductName, decimal Price, int Quantity);

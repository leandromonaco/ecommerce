namespace ECommerce.Shared.DTOs;

public record PaymentMessage(int OrderId, decimal Amount, Guid PaymentId);

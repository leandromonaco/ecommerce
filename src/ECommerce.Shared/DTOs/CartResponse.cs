using ECommerce.Shared.Models;

namespace ECommerce.Shared.DTOs;

public record CartResponse(List<CartItem> Items, decimal Total);

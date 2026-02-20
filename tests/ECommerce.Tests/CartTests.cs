using ECommerce.Shared.Models;

namespace ECommerce.Tests;

public class CartTests
{
    [Fact]
    public void CartItem_ShouldHaveDefaultValues()
    {
        var item = new CartItem();

        Assert.Equal(0, item.Id);
        Assert.Equal(string.Empty, item.ProductName);
        Assert.Equal(0m, item.Price);
        Assert.Equal(0, item.Quantity);
        Assert.Equal(string.Empty, item.CartId);
    }

    [Fact]
    public void CartItem_ShouldSetProperties()
    {
        var item = new CartItem
        {
            Id = 1,
            ProductName = "Widget",
            Price = 9.99m,
            Quantity = 2,
            CartId = "cart-1"
        };

        Assert.Equal("Widget", item.ProductName);
        Assert.Equal(9.99m, item.Price);
        Assert.Equal(2, item.Quantity);
    }

    // TODO: Add tests for cart total calculation
    // TODO: Add tests for checkout flow (create order, publish message, clear cart)
    // TODO: Add integration tests using WebApplicationFactory
}

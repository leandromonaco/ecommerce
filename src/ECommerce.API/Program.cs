using ECommerce.Shared.Data;
using ECommerce.Shared.DTOs;
using ECommerce.Shared.Models;
using ECommerce.Shared.Queue;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(DatabaseConfiguration.ConnectionString));

builder.Services.AddSingleton<SqliteMessageQueue>();
builder.Services.AddSingleton<IMessageQueue>(sp => sp.GetRequiredService<SqliteMessageQueue>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

const string defaultCartId = "default-cart";

// POST /api/cart/items - Add item to cart
app.MapPost("/api/cart/items", async (AddToCartRequest request, AppDbContext db) =>
{
    var item = new CartItem
    {
        ProductName = request.ProductName,
        Price = request.Price,
        Quantity = request.Quantity,
        CartId = defaultCartId
    };

    db.CartItems.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/api/cart/items/{item.Id}", item);
});

// DELETE /api/cart/items/{id} - Remove item from cart
app.MapDelete("/api/cart/items/{id}", async (int id, AppDbContext db) =>
{
    var item = await db.CartItems.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.CartItems.Remove(item);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// PUT /api/cart/items/{id} - Update item quantity
app.MapPut("/api/cart/items/{id}", async (int id, UpdateCartItemRequest request, AppDbContext db) =>
{
    var item = await db.CartItems.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Quantity = request.Quantity;
    await db.SaveChangesAsync();

    return Results.Ok(item);
});

// GET /api/cart - Get cart with items and total
app.MapGet("/api/cart", async (AppDbContext db) =>
{
    var items = await db.CartItems
        .Where(i => i.CartId == defaultCartId)
        .ToListAsync();

    var total = items.Sum(i => i.Price * i.Quantity);

    return Results.Ok(new CartResponse(items, total));
});

// POST /api/cart/checkout - Create order from cart and publish payment message
app.MapPost("/api/cart/checkout", async (AppDbContext db, IMessageQueue queue) =>
{
    var items = await db.CartItems
        .Where(i => i.CartId == defaultCartId)
        .ToListAsync();

    if (!items.Any())
        return Results.BadRequest("Cart is empty");

    return Results.Ok();
});

// GET /api/orders/{id} - Get order status
app.MapGet("/api/orders/{id}", async (int id, AppDbContext db) =>
{
    var order = await db.Orders.FindAsync(id);
    if (order is null) return Results.NotFound();

    return Results.Ok(new OrderResponse(
        order.Id,
        order.Status.ToString(),
        order.TotalAmount,
        order.CreatedAt));
});

app.Run();

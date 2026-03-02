import { useEffect, useState } from "react";
import { AddItemForm } from "./components/AddItemForm";
import { CartTable } from "./components/CartTable";
import { OrderStatus } from "./components/OrderStatus";
import type { CartItem } from "./models/CartItem";
import type { OrderResponse } from "./models/Order";
import * as api from "./services/api";
import "./App.css";

function App() {
  const [items, setItems] = useState<CartItem[]>([]);
  const [total, setTotal] = useState(0);
  const [order, setOrder] = useState<OrderResponse | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  useEffect(() => {
    let cancelled = false;
    api.getCart().then((cart) => {
      if (!cancelled) {
        setItems(cart.items);
        setTotal(cart.total);
      }
    }).catch(() => {
      if (!cancelled) setError("Failed to load cart");
    });
    return () => { cancelled = true; };
  }, [refreshKey]);

  const reloadCart = () => setRefreshKey((k) => k + 1);

  const handleAdd = async (
    productName: string,
    price: number,
    quantity: number
  ) => {
    try {
      await api.addToCart(productName, price, quantity);
      reloadCart();
    } catch {
      setError("Failed to add item");
    }
  };

  const handleRemove = async (id: number) => {
    try {
      await api.removeFromCart(id);
      reloadCart();
    } catch {
      setError("Failed to remove item");
    }
  };

  const handleUpdateQuantity = async (id: number, quantity: number) => {
    try {
      await api.updateCartItem(id, quantity);
      reloadCart();
    } catch {
      setError("Failed to update item");
    }
  };

  const handleCheckout = async () => {
    try {
      const result = await api.checkout();
      setOrder(result);
      await loadCart();
    } catch {
      setError("Failed to complete checkout");
    }
  };

  const handleRefreshOrder = async () => {
    if (!order) return;
    try {
      const updated = await api.getOrder(order.orderId);
      setOrder(updated);
    } catch {
      setError("Failed to refresh order status");
    }
  };

  return (
    <div style={{ maxWidth: "800px", margin: "0 auto", padding: "2rem" }}>
      <h1>🛒 Shopping Cart</h1>

      {error && (
        <div
          style={{
            color: "red",
            padding: "0.5rem",
            marginBottom: "1rem",
            border: "1px solid red",
            borderRadius: "4px",
          }}
        >
          {error}
          <button
            onClick={() => setError(null)}
            style={{ marginLeft: "1rem" }}
          >
            Dismiss
          </button>
        </div>
      )}

      <AddItemForm onAdd={handleAdd} />

      <CartTable
        items={items}
        total={total}
        onRemove={handleRemove}
        onUpdateQuantity={handleUpdateQuantity}
      />

      {items.length > 0 && (
        <button
          onClick={handleCheckout}
          style={{
            marginTop: "1rem",
            padding: "0.5rem 2rem",
            fontSize: "1.1rem",
            backgroundColor: "#4CAF50",
            color: "white",
            border: "none",
            borderRadius: "4px",
            cursor: "pointer",
          }}
        >
          Checkout
        </button>
      )}

      <OrderStatus order={order} onRefresh={handleRefreshOrder} />
    </div>
  );
}

export default App;

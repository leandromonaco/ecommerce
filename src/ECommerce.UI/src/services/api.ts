import type { CartResponse } from "../models/CartResponse";
import type { OrderResponse } from "../models/Order";

const API_BASE = "http://localhost:53001/api";

export async function getCart(): Promise<CartResponse> {
  const response = await fetch(`${API_BASE}/cart`);
  return response.json();
}

export async function addToCart(
  productName: string,
  price: number,
  quantity: number
): Promise<void> {
  await fetch(`${API_BASE}/cart/items`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ productName, price, quantity }),
  });
}

export async function removeFromCart(id: number): Promise<void> {
  await fetch(`${API_BASE}/cart/items/${id}`, { method: "DELETE" });
}

export async function updateCartItem(
  id: number,
  quantity: number
): Promise<void> {
  await fetch(`${API_BASE}/cart/items/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ quantity }),
  });
}

export async function checkout(): Promise<OrderResponse> {
  const response = await fetch(`${API_BASE}/cart/checkout`, {
    method: "POST",
  });
  return response.json();
}

export async function getOrder(id: number): Promise<OrderResponse> {
  const response = await fetch(`${API_BASE}/orders/${id}`);
  return response.json();
}

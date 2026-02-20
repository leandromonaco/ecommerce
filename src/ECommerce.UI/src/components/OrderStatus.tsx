import type { OrderResponse } from "../models/Order";

interface OrderStatusProps {
  order: OrderResponse | null;
  onRefresh: () => void;
}

export function OrderStatus({ order, onRefresh }: OrderStatusProps) {
  if (!order) return null;

  const statusColor =
    order.status === "Paid"
      ? "green"
      : order.status === "Failed"
        ? "red"
        : "orange";

  return (
    <div
      style={{
        marginTop: "1rem",
        padding: "1rem",
        border: "1px solid #ddd",
        borderRadius: "4px",
      }}
    >
      <h3>Order #{order.orderId}</h3>
      <p>
        Status:{" "}
        <span style={{ color: statusColor, fontWeight: "bold" }}>
          {order.status}
        </span>
      </p>
      <p>Total: ${order.totalAmount.toFixed(2)}</p>
      <p>Created: {new Date(order.createdAt).toLocaleString()}</p>
      {(order.status === "Pending" ||
        order.status === "PaymentProcessing") && (
        <button onClick={onRefresh}>Refresh Status</button>
      )}
    </div>
  );
}

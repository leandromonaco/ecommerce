import type { CartItem } from "../models/CartItem";

interface CartTableProps {
  items: CartItem[];
  total: number;
  onRemove: (id: number) => void;
  onUpdateQuantity: (id: number, quantity: number) => void;
}

export function CartTable({
  items,
  total,
  onRemove,
  onUpdateQuantity,
}: CartTableProps) {
  if (items.length === 0) {
    return <p>Your cart is empty.</p>;
  }

  return (
    <div>
      <table style={{ width: "100%", borderCollapse: "collapse" }}>
        <thead>
          <tr>
            <th style={thStyle}>Product</th>
            <th style={thStyle}>Price</th>
            <th style={thStyle}>Quantity</th>
            <th style={thStyle}>Subtotal</th>
            <th style={thStyle}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.id}>
              <td style={tdStyle}>{item.productName}</td>
              <td style={tdStyle}>${item.price.toFixed(2)}</td>
              <td style={tdStyle}>
                {/* TODO: Add debouncing or a confirm button for quantity updates */}
                <input
                  type="number"
                  min={1}
                  value={item.quantity}
                  onChange={(e) =>
                    onUpdateQuantity(item.id, Number(e.target.value))
                  }
                  style={{ width: "60px" }}
                />
              </td>
              <td style={tdStyle}>
                ${(item.price * item.quantity).toFixed(2)}
              </td>
              <td style={tdStyle}>
                <button onClick={() => onRemove(item.id)}>Remove</button>
              </td>
            </tr>
          ))}
        </tbody>
        <tfoot>
          <tr>
            <td colSpan={3} style={{ ...tdStyle, fontWeight: "bold" }}>
              Total
            </td>
            <td style={{ ...tdStyle, fontWeight: "bold" }}>
              ${total.toFixed(2)}
            </td>
            <td style={tdStyle}></td>
          </tr>
        </tfoot>
      </table>
    </div>
  );
}

const thStyle: React.CSSProperties = {
  borderBottom: "2px solid #333",
  padding: "8px",
  textAlign: "left",
};

const tdStyle: React.CSSProperties = {
  borderBottom: "1px solid #ddd",
  padding: "8px",
};

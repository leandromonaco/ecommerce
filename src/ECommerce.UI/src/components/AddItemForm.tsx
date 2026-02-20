import { useState } from "react";

interface AddItemFormProps {
  onAdd: (productName: string, price: number, quantity: number) => void;
}

const SAMPLE_PRODUCTS = [
  { name: "Widget", price: 9.99 },
  { name: "Gadget", price: 24.99 },
  { name: "Doohickey", price: 14.99 },
  { name: "Thingamajig", price: 49.99 },
];

export function AddItemForm({ onAdd }: AddItemFormProps) {
  const [selectedProduct, setSelectedProduct] = useState(0);
  const [quantity, setQuantity] = useState(1);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const product = SAMPLE_PRODUCTS[selectedProduct];
    onAdd(product.name, product.price, quantity);
    setQuantity(1);
  };

  return (
    <form onSubmit={handleSubmit} style={{ marginBottom: "1rem" }}>
      <h3>Add Item</h3>
      <div style={{ display: "flex", gap: "0.5rem", alignItems: "center" }}>
        <select
          value={selectedProduct}
          onChange={(e) => setSelectedProduct(Number(e.target.value))}
        >
          {SAMPLE_PRODUCTS.map((p, i) => (
            <option key={p.name} value={i}>
              {p.name} - ${p.price.toFixed(2)}
            </option>
          ))}
        </select>
        <input
          type="number"
          min={1}
          value={quantity}
          onChange={(e) => setQuantity(Number(e.target.value))}
          style={{ width: "60px" }}
        />
        <button type="submit">Add to Cart</button>
      </div>
    </form>
  );
}

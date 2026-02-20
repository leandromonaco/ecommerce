import type { CartItem } from "./CartItem";

export interface CartResponse {
  items: CartItem[];
  total: number;
}

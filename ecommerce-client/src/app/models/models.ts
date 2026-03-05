export interface User {
  id: string;
  username: string;
  email: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expiration?: Date;
  user: User;
}

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  stock: number;
  stockQuantity: number; // Alias for stock
  categoryId?: string;
  imageUrl?: string;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CartItem {
  id: string;
  productId: string;
  quantity: number;
  priceAtTimeOfAddition: number;
  addedAt: Date;
}

export interface Cart {
  id: string;
  userId: string;
  items: CartItem[];
  createdAt: Date;
  updatedAt?: Date;
}

export interface Order {
  id: string;
  createdBy: string;
  createdAt: Date;
  orderItems: OrderItem[];
}

export interface OrderItem {
  id: string;
  orderId: string;
  productId: string;
  productName: string;
  product?: Product;
  quantity: number;
  price: number;
}

export interface Payment {
  id: string;
  orderId: string;
  amount: number;
  currency: string;
  status: string;
  stripePaymentIntentId?: string;
  createdAt: Date;
}

export interface PaymentRequest {
  orderId: string;
  userId: string;
  amount: number;
  currency: string;
  paymentMethodId: string;
}

import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { PaymentService } from '../../services/payment.service';
import { Order, Payment } from '../../models/models';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit {
  private cartService = inject(CartService);
  private orderService = inject(OrderService);
  private paymentService = inject(PaymentService);
  private router = inject(Router);

  cart$ = this.cartService.cart$;
  loading = signal(false);
  error = signal<string | null>(null);

  // Shipping information
  shippingAddress = {
    street: '',
    city: '',
    state: '',
    zipCode: '',
    country: ''
  };

  // Payment information (mock for prototype)
  cardNumber = '';
  cardExpiry = '';
  cardCvv = '';
  cardholderName = '';

  ngOnInit(): void {
    this.cartService.getCart().subscribe();
  }

  calculateTotal(cart: any): number {
    return cart.items.reduce((sum: number, item: any) => sum + (item.priceAtTimeOfAddition * item.quantity), 0);
  }

  async placeOrder(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      // Get current cart
      const currentCart = await new Promise<any>((resolve, reject) => {
        this.cartService.getCart().subscribe({
          next: (cart) => resolve(cart),
          error: (err) => reject(err)
        });
      });

      if (!currentCart || !currentCart.id) {
        this.error.set('Cart not found');
        this.loading.set(false);
        return;
      }

      // Create order with cartId
      const order = await new Promise<any>((resolve, reject) => {
        this.orderService.createOrder(currentCart.id).subscribe({
          next: (response) => resolve(response),
          error: (err: any) => reject(err)
        });
      });

      if (order) {
        const orderDetails = order.order || order;
        const totalAmount = this.calculateTotal(currentCart);

        // Process payment
        const paymentRequest = {
          orderId: orderDetails.id,
          userId: currentCart.userId,
          amount: totalAmount,
          currency: 'USD',
          paymentMethodId: 'mock_payment_method'
        };

        const payment = await new Promise<any>((resolve, reject) => {
          this.paymentService.processPayment(paymentRequest).subscribe({
            next: (response) => resolve(response),
            error: (err) => reject(err)
          });
        });

        if (payment?.success) {
          // Clear cart and navigate to orders page
          await new Promise<void>((resolve, reject) => {
            this.cartService.clearCart().subscribe({
              next: () => resolve(),
              error: (err: any) => reject(err)
            });
          });
          this.router.navigate(['/orders']);
        } else {
          this.error.set('Payment failed. Please try again.');
        }
      } else {
        this.error.set('Failed to create order. Please try again.');
      }
    } catch (err: any) {
      this.error.set(err.error?.message || 'An error occurred during checkout.');
    } finally {
      this.loading.set(false);
    }
  }

  isFormValid(): boolean {
    return !!(
      this.shippingAddress.street &&
      this.shippingAddress.city &&
      this.shippingAddress.state &&
      this.shippingAddress.zipCode &&
      this.shippingAddress.country &&
      this.cardholderName &&
      this.cardNumber &&
      this.cardExpiry &&
      this.cardCvv
    );
  }
}

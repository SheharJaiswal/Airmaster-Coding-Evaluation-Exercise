import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { Cart, CartItem } from '../../models/models';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  private cartService = inject(CartService);
  private router = inject(Router);

  cart$ = this.cartService.cart$;
  loading = signal(false);

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.loading.set(true);
    this.cartService.getCart().subscribe({
      next: () => this.loading.set(false),
      error: () => this.loading.set(false)
    });
  }

  updateQuantity(item: CartItem, quantity: number): void {
    if (quantity < 1) return;
    
    this.cartService.updateCartItem(item.productId, quantity).subscribe({
      error: (err) => {
        alert('Failed to update quantity');
      }
    });
  }

  removeItem(item: CartItem): void {
    if (confirm('Remove this item from cart?')) {
      this.cartService.removeFromCart(item.productId).subscribe({
        error: (err) => {
          alert('Failed to remove item');
        }
      });
    }
  }

  clearCart(): void {
    if (confirm('Clear all items from cart?')) {
      this.cartService.clearCart().subscribe({
        error: (err: any) => {
          alert('Failed to clear cart');
        }
      });
    }
  }

  proceedToCheckout(): void {
    this.router.navigate(['/checkout']);
  }

  calculateTotal(cart: Cart): number {
    return cart.items.reduce((sum, item) => sum + (item.priceAtTimeOfAddition * item.quantity), 0);
  }
}

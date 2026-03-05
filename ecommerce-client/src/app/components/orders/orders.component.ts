import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { Order } from '../../models/models';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  private orderService = inject(OrderService);

  orders = signal<Order[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit(): void {
    // Backend doesn't have getUserOrders endpoint yet
    // This component is a placeholder for future implementation
    this.loading.set(false);
  }

  getOrderById(orderId: string): void {
    this.loading.set(true);
    this.orderService.getOrder(orderId).subscribe({
      next: (order) => {
        this.orders.set([order]);
        this.loading.set(false);
      },
      error: (err: any) => {
        this.error.set('Failed to load order');
        this.loading.set(false);
      }
    });
  }
}

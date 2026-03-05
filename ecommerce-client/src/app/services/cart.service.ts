import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { Cart, CartItem } from '../models/models';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private cartSubject = new BehaviorSubject<Cart | null>(null);
  public cart$ = this.cartSubject.asObservable();

  constructor() {}

  private getUserId(): string {
    return this.authService.currentUserValue?.id || '';
  }

  getCart(): Observable<Cart> {
    const userId = this.getUserId();
    const url = `${environment.apiEndpoints.cartServiceUrl}/${userId}`;
    return this.http.get<Cart>(url).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  addToCart(productId: string, quantity: number = 1): Observable<Cart> {
    const userId = this.getUserId();
    const url = `${environment.apiEndpoints.cartServiceUrl}/${userId}/items`;
    return this.http.post<Cart>(url, { productId, quantity }).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  updateCartItem(productId: string, newQuantity: number): Observable<void> {
    const userId = this.getUserId();
    const url = `${environment.apiEndpoints.cartServiceUrl}/${userId}/items/${productId}`;
    return this.http.put<void>(url, { newQuantity });
  }

  removeFromCart(productId: string): Observable<void> {
    const userId = this.getUserId();
    const url = `${environment.apiEndpoints.cartServiceUrl}/${userId}/items/${productId}`;
    return this.http.delete<void>(url);
  }

  clearCart(): Observable<void> {
    const userId = this.getUserId();
    const url = `${environment.apiEndpoints.cartServiceUrl}/${userId}`;
    return this.http.delete<void>(url).pipe(
      tap(() => this.cartSubject.next(null))
    );
  }

  get cartItemCount(): number {
    const cart = this.cartSubject.value;
    return cart ? cart.items.reduce((sum, item) => sum + item.quantity, 0) : 0;
  }
}

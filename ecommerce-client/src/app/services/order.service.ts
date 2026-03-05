import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Order } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private http = inject(HttpClient);

  constructor() {}

  createOrder(cartId: string): Observable<any> {
    const url = `${environment.apiEndpoints.orderServiceUrl}/${cartId}`;
    return this.http.post<any>(url, {});
  }

  getOrder(orderId: string): Observable<Order> {
    const url = `${environment.apiEndpoints.orderServiceUrl}/${orderId}`;
    return this.http.get<Order>(url);
  }

  getTrackingInfo(trackingNumber: string, carrier: string = 'FedEx'): Observable<any> {
    const url = `${environment.apiEndpoints.orderServiceUrl}/tracking/${trackingNumber}?carrier=${carrier}`;
    return this.http.get<any>(url);
  }

  cancelShipment(trackingNumber: string): Observable<any> {
    const url = `${environment.apiEndpoints.orderServiceUrl}/shipment/${trackingNumber}`;
    return this.http.delete<any>(url);
  }
}

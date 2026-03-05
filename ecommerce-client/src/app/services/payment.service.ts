import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Payment, PaymentRequest } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private http = inject(HttpClient);

  constructor() {}

  processPayment(paymentRequest: PaymentRequest): Observable<any> {
    const url = `${environment.apiEndpoints.paymentServiceUrl}/process`;
    return this.http.post<any>(url, paymentRequest);
  }

  getPaymentByOrderId(orderId: string): Observable<Payment> {
    const url = `${environment.apiEndpoints.paymentServiceUrl}/order/${orderId}`;
    return this.http.get<Payment>(url);
  }

  refundPayment(paymentId: string): Observable<any> {
    const url = `${environment.apiEndpoints.paymentServiceUrl}/refund/${paymentId}`;
    return this.http.post<any>(url, {});
  }
}

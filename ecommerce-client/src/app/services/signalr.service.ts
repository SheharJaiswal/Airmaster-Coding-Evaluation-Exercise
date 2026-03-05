import { Injectable, inject, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

export interface OrderStatusUpdate {
  orderId: string;
  status: string;
  message: string;
  timestamp: Date;
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private authService = inject(AuthService);
  private hubConnection: signalR.HubConnection | null = null;
  
  public connectionState = signal<'disconnected' | 'connecting' | 'connected'>('disconnected');
  public orderUpdates = signal<OrderStatusUpdate[]>([]);

  constructor() {}

  startConnection(): Promise<void> {
    const token = this.authService.getToken();
    if (!token) {
      return Promise.reject('No authentication token available');
    }

    this.connectionState.set('connecting');

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRUrl, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('OrderStatusChanged', (data: OrderStatusUpdate) => {
      this.orderUpdates.update(updates => [...updates, data]);
    });

    this.hubConnection.on('UserNotification', (data: any) => {
      console.log('User notification:', data);
    });

    this.hubConnection.onreconnecting(() => {
      this.connectionState.set('connecting');
    });

    this.hubConnection.onreconnected(() => {
      this.connectionState.set('connected');
    });

    this.hubConnection.onclose(() => {
      this.connectionState.set('disconnected');
    });

    return this.hubConnection
      .start()
      .then(() => {
        this.connectionState.set('connected');
        console.log('SignalR connected');
      })
      .catch(err => {
        this.connectionState.set('disconnected');
        console.error('SignalR connection error:', err);
        throw err;
      });
  }

  subscribeToOrder(orderId: string): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('Hub connection not established');
    }

    return this.hubConnection.invoke('SubscribeToOrder', orderId);
  }

  unsubscribeFromOrder(orderId: string): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('Hub connection not established');
    }

    return this.hubConnection.invoke('UnsubscribeFromOrder', orderId);
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = null;
      this.connectionState.set('disconnected');
    }
  }
}

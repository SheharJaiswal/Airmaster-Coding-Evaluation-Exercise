import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { Product } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private http = inject(HttpClient);
  private productsSubject = new BehaviorSubject<Product[]>([]);
  public products$ = this.productsSubject.asObservable();

  constructor() {}

  getProducts(): Observable<Product[]> {
    const url = `${environment.apiEndpoints.productServiceUrl}`;
    return this.http.get<Product[]>(url).pipe(
      tap(products => this.productsSubject.next(products))
    );
  }

  getProductsByCategory(category: string): Observable<Product[]> {
    const url = `${environment.apiEndpoints.productServiceUrl}/category/${category}`;
    return this.http.get<Product[]>(url).pipe(
      tap(products => this.productsSubject.next(products))
    );
  }

  getProduct(id: string): Observable<Product> {
    const url = `${environment.apiEndpoints.productServiceUrl}/${id}`;
    return this.http.get<Product>(url);
  }

  createProduct(product: Partial<Product>): Observable<Product> {
    const url = `${environment.apiEndpoints.productServiceUrl}`;
    return this.http.post<Product>(url, product);
  }

  updateProduct(id: string, product: Partial<Product>): Observable<Product> {
    const url = `${environment.apiEndpoints.productServiceUrl}/${id}`;
    return this.http.put<Product>(url, product);
  }

  deleteProduct(id: string): Observable<void> {
    const url = `${environment.apiEndpoints.productServiceUrl}/${id}`;
    return this.http.delete<void>(url);
  }
}

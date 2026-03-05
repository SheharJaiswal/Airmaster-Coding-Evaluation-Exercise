import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { Product } from '../../models/models';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {
  private productService = inject(ProductService);
  private cartService = inject(CartService);

  products$ = this.productService.products$;
  loading = signal(true);
  searchTerm = signal('');
  selectedCategory = signal('');

  categories = ['Electronics', 'Clothing', 'Books', 'Home & Garden', 'Sports'];

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading.set(true);
    const category = this.selectedCategory();
    
    if (category) {
      this.productService.getProductsByCategory(category).subscribe({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      });
    } else {
      this.productService.getProducts().subscribe({
        next: () => this.loading.set(false),
        error: () => this.loading.set(false)
      });
    }
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
    this.loadProducts();
  }

  onCategoryChange(category: string): void {
    this.selectedCategory.set(category);
    this.loadProducts();
  }

  addToCart(product: Product): void {
    this.cartService.addToCart(product.id, 1).subscribe({
      next: () => {
        alert('Product added to cart!');
      },
      error: (err) => {
        alert('Failed to add product to cart');
      }
    });
  }
}

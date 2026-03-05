# E-Commerce Angular Client

Modern Angular client application for the E-Commerce microservices platform with real-time order tracking, payment processing, and responsive SCSS styling.

## Features

- 🔐 **JWT Authentication** - Secure login/register with token-based authentication
- 🛍️ **Product Catalog** - Browse products with search and category filtering
- 🛒 **Shopping Cart** - Real-time cart management with quantity controls
- 💳 **Checkout Process** - Integrated payment processing with Stripe
- 📦 **Order Tracking** - Real-time order status updates via SignalR WebSockets
- 🎨 **Modern UI/UX** - Responsive design with SCSS styling and smooth animations
- 📱 **Mobile Responsive** - Fully responsive design for all screen sizes

## Tech Stack

- **Angular 20.1+** - Latest Angular with zoneless architecture
- **TypeScript** - Type-safe development
- **SCSS** - Advanced styling with variables and mixins
- **RxJS** - Reactive programming with Observables
- **SignalR** - Real-time WebSocket communication
- **Standalone Components** - Modern Angular component architecture
- **Signals API** - Angular's new reactive primitives

## Prerequisites

- **Node.js** 20.x or higher
- **npm** 10.x or higher
- **Angular CLI** 20.x

## Installation

1. **Navigate to the client directory:**
   ```powershell
   cd ecommerce-client
   ```

2. **Install dependencies:**
   ```powershell
   npm install
   ```

3. **Configure environment:**
   Edit `src/environments/environment.ts` to point to your API Gateway:
   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'http://localhost:9005/api',
     signalrUrl: 'http://localhost:8005',
     stripePublishableKey: 'your_stripe_key_here'
   };
   ```

## Running the Application

### Development Server

```powershell
npm start
```

Navigate to `http://localhost:4200/`. The application will automatically reload when source files change.

### Production Build

```powershell
npm run build
```

Build artifacts will be stored in the `dist/` directory.

## Project Structure

```
src/app/
├── components/
│   ├── header/          # Navigation header with cart badge
│   ├── login/           # Login form
│   ├── register/        # Registration form
│   ├── products/        # Product listing with search/filter
│   ├── cart/            # Shopping cart
│   ├── checkout/        # Checkout with payment
│   └── orders/          # Order history with real-time updates
├── services/            # API services and state management
├── interceptors/        # HTTP interceptors (auth, error)
├── guards/              # Route guards
├── models/              # TypeScript interfaces
└── environments/        # Configuration files
```

## API Integration

The Angular client integrates with microservices via API Gateway (`http://localhost:9005/api`):

- **UserService** - Authentication, user management
- **ProductService** - Product catalog, search
- **CartService** - Shopping cart operations
- **OrderService** - Order creation, tracking
- **PaymentService** - Payment processing
- **SignalR Hub** - Real-time order status updates

## Features Overview

### Authentication
- JWT-based authentication with localStorage
- Auto-logout on token expiration
- Protected routes with auth guard

### Shopping Flow
1. Browse products with search/filter
2. Add items to cart
3. Checkout with shipping info
4. Process payment (prototype mode)
5. Track order with real-time updates

### Styling
- Modern gradient themes (purple-violet)
- Smooth animations and transitions
- Form validation states
- Responsive grid layouts
- Mobile-first design

## Security

- JWT tokens in Authorization headers
- Rate limiting (100 req/min)
- CORS configuration
- Input validation

## Troubleshooting

### CORS Issues
Ensure API Gateway allows `http://localhost:4200` in CORS policy.

### SignalR Connection Fails
1. Verify OrderService is running on port 8005
2. Check SignalR URL in environment.ts
3. Ensure JWT token is valid

---

**Happy Coding! 🚀**

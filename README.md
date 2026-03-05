# E-Commerce Microservices Platform

A scalable, production-ready e-commerce platform built with microservices architecture using ASP.NET Core 8.0 and Angular 20.1+.

## 📋 Project Structure

```
ecommerce-service/
├── README.md                    # Main project documentation
├── ecommerce-client/            # Angular 20.1+ frontend
│   ├── README.md               # Frontend documentation
│   └── src/
│       ├── app/                # Angular components, services, guards
│       └── environments/        # Configuration files
└── services-ecommerce/         # .NET backend microservices
    ├── ApiGateway/            # Ocelot API Gateway
    ├── UserService/           # Authentication & user management
    ├── ProductService/        # Product catalog with caching
    ├── CartService/           # Shopping cart management
    ├── OrderService/          # Order processing with real-time updates
    ├── PaymentService/        # Stripe payment processing
    ├── Shared/                # Shared libraries
    └── deploy/                # Docker configuration
```

## 🎯 Key Features

✅ **Microservices Architecture** - 6 independent services + API Gateway
✅ **JWT Authentication** - Secure token-based authentication
✅ **Real-Time Updates** - SignalR WebSocket for order tracking
✅ **Payment Processing** - Stripe integration with circuit breakers
✅ **Caching Layer** - Redis for product catalog optimization
✅ **Message Queue** - RabbitMQ with dead-letter queues
✅ **Service Discovery** - Eureka server for dynamic registration
✅ **Shipping Integration** - Mock FedEx/UPS API
✅ **Rate Limiting** - 100 requests/minute per user
✅ **Docker Ready** - Complete Docker Compose setup

## 🚀 Quick Start

### Prerequisites

- **Docker Desktop** (Windows/Mac) or Docker Engine (Linux)
- **Node.js** 20.x or higher (for Angular CLI)
- **.NET 8.0 SDK** (optional, for local development)

### Option 1: Docker Compose (Recommended)

```powershell
# Navigate to deploy directory
cd services-ecommerce\deploy

# Build and start all services
docker-compose up --build

# Wait for services to be healthy (2-3 minutes)
# Check status: docker-compose ps
```

All services will be available on these ports:
- **API Gateway**: http://localhost:9005
- **UserService**: http://localhost:5005
- **ProductService**: http://localhost:6005
- **CartService**: http://localhost:7005
- **OrderService**: http://localhost:8005
- **PaymentService**: http://localhost:5010
- **Eureka Dashboard**: http://localhost:8761
- **MySQL Database**: localhost:3306
- **Redis Cache**: localhost:6379
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

### Option 2: Angular Frontend

```powershell
# Navigate to frontend directory
cd ecommerce-client

# Install dependencies
npm install

# Start development server
npm start

# Open browser to http://localhost:4200
```

## 📚 Documentation

### Frontend
See [ecommerce-client/README.md](./ecommerce-client/README.md) for:
- Angular architecture
- Component structure
- Service integration
- Authentication flow
- Build instructions

### Backend
See [services-ecommerce/README.md](./services-ecommerce/README.md) for:
- Service setup instructions
- API endpoints
- Local development setup
- Testing procedures
- Environment configuration

## 🏗️ Architecture Overview

### Layered Architecture

```
┌─────────────────────────────────────────┐
│        Angular Client (Port 4200)        │
└────────────────────┬────────────────────┘
                     │ HTTPS
                     ▼
┌─────────────────────────────────────────┐
│   API Gateway - Ocelot (Port 9005)      │
│  • Rate Limiting (100 req/min)          │
│  • JWT Authentication                   │
│  • Request Routing & Load Balancing     │
└────────┬───────────────────────────────┘
         │
    ┌────┴─────────────────────────┬──────────┬──────────────┐
    ▼                              ▼          ▼              ▼
┌────────────┐  ┌──────────────┐ ┌────────┐ ┌────────────┐ ┌──────────┐
│UserService │  │ProductService│ │CartSvc │ │OrderService│ │PaymentSvc│
│(Auth)      │  │(+ Redis)     │ │        │ │(+ SignalR) │ │(Stripe)  │
│Port: 5005  │  │Port: 6005    │ │Port:7k │ │Port: 8005  │ │Port:5010 │
└────────┬───┘  └──────┬───────┘ └───┬────┘ └────────────┘ └──────────┘
         │             │             │
         └─────────────┼─────────────┘
                       ▼
            ┌──────────────────────┐
            │   MySQL Database     │
            │   (Port 3306)        │
            └──────────────────────┘
                       ▲
         ┌─────────────┼─────────────┐
         ▼             ▼             ▼
    ┌────────┐    ┌──────────┐  ┌────────┐
    │ Redis  │    │ RabbitMQ │  │ Eureka │
    │(Cache) │    │(Messages)│  │(Discov)│
    └────────┘    └──────────┘  └────────┘
```

### Service Responsibilities

| Service | Port | Responsibility |
|---------|------|-----------------|
| **UserService** | 5005 | JWT authentication, user registration, login |
| **ProductService** | 6005 | Product catalog, search, Redis caching |
| **CartService** | 7005 | Shopping cart CRUD, cart item management |
| **OrderService** | 8005 | Order creation, tracking, SignalR notifications |
| **PaymentService** | 5010 | Stripe payment processing, refunds, circuit breaker |
| **API Gateway** | 9005 | Request routing, rate limiting, authentication |

## 🔐 Security Features

- **JWT Authentication**: Secure token-based access control
- **CORS Configuration**: Configured for frontend communication
- **Rate Limiting**: 100 requests/minute per user
- **Circuit Breakers**: Fault tolerance for external services
- **Password Hashing**: BCrypt with salt for secure storage
- **PCI-DSS Compliance**: Stripe handles payment card security

## 📊 Database Schema

### Centralized Database
All services share a single MySQL database (`Ecommerce`) with separate tables per service:

```
Ecommerce Database
├── Users (UserService)
├── Products (ProductService)
├── Carts (CartService)
├── CartItems (CartService)
├── Orders (OrderService)
├── OrderItems (OrderService)
└── Payments (PaymentService)
```

**Migrations**: Automatically applied on service startup via centralized `MigrationInitializer`

## 🔄 Data Flow Examples

### User Registration & Login
```
1. User fills registration form → Angular component
2. POST /api/Auth/register → API Gateway
3. Route to UserService:5005
4. Hash password → Store in MySQL
5. Return JWT token + user data
6. Store token in localStorage
7. Redirect to products page
```

### Add to Cart
```
1. User clicks "Add to Cart" → Product component
2. Get current user from localStorage
3. POST /api/Carts/{userId}/items → API Gateway
4. Route to CartService:7005
5. Validate product exists (ProductService check)
6. Create CartItem in MySQL
7. Return updated cart
8. Update UI with new item
```

### Checkout & Payment
```
1. User clicks "Checkout" → Checkout component
2. POST /api/Order/create → API Gateway
3. OrderService creates order record
4. Emit OrderCreated event → RabbitMQ
5. PaymentService consumes event
6. POST to Stripe API
7. Return payment status
8. OrderService updates order status
9. Emit OrderStatusChanged → SignalR
10. Angular receives real-time update
11. Redirect to order confirmation
```

## 🧪 Testing the System

### Test User Credentials (Seeded in Database)

```
Email: admin@test.com
Password: Admin123!
```

### API Gateway Testing

```powershell
# Login
$response = Invoke-RestMethod -Uri "http://localhost:9005/api/Auth/login" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"email":"admin@test.com","password":"Admin123!"}'

$token = $response.token

# Get Products
Invoke-RestMethod -Uri "http://localhost:9005/api/Products" `
  -Method GET `
  -Headers @{ Authorization = "Bearer $token" }

# Get Cart
Invoke-RestMethod -Uri "http://localhost:9005/api/Carts/{userId}" `
  -Method GET `
  -Headers @{ Authorization = "Bearer $token" }
```

### SignalR Real-Time Testing

```javascript
// Connect to order updates
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:9005/hubs/orderstatus", {
        accessTokenFactory: () => localStorage.getItem('auth_token')
    })
    .withAutomaticReconnect()
    .build();

connection.on("OrderStatusChanged", (data) => {
    console.log("Order updated:", data);
});

await connection.start();
await connection.invoke("SubscribeToOrder", "order-id");
```

## 🐛 Troubleshooting

### Services Not Starting

```powershell
# Check service status
docker-compose ps

# View service logs
docker-compose logs service-name

# Restart all services
docker-compose restart

# Full reset (warning: deletes data)
docker-compose down -v
docker-compose up --build
```

### Database Connection Issues

```powershell
# Test database connectivity
docker exec deploy-new-ecommerce-db-1 mysql -uroot -proot123 -e "SHOW DATABASES;"

# Check migrations
docker-compose logs --grep "Successfully migrated"
```

### API Gateway Routing Issues

Check `services-ecommerce/ApiGateway/ocelot.json` for:
- Correct service names in downstream hosts
- Proper route paths
- Authentication configuration

### Redis Cache Issues

```powershell
# Connect to Redis
docker exec -it deploy-redis-1 redis-cli

# Check cached keys
KEYS *

# View specific cache entry
GET ProductCache:product:id
```

## 🔧 Configuration

### Environment Variables

Create `.env` file in `deploy/` directory:

```env
# MySQL
MYSQL_ROOT_PASSWORD=root123
MYSQL_DATABASE=Ecommerce

# JWT
JWT_KEY=your-super-secret-key-min-32-chars-long!
JWT_ISSUER=UserService
JWT_AUDIENCE=MyApp

# Stripe (Get from https://dashboard.stripe.com)
STRIPE_SECRET_KEY=sk_test_your_key_here
STRIPE_PUBLISHABLE_KEY=pk_test_your_key_here

# Redis
REDIS_PASSWORD=your-redis-password

# API Gateway
RATE_LIMIT=100
```

### Service Configuration Files

Each service has `appsettings.json`:
- `appsettings.json` - Shared configuration
- `appsettings.Development.json` - Local development
- `appsettings.Docker.json` - Docker container settings

## 📈 Scalability

### Horizontal Scaling

Each service can run multiple instances:

```yaml
# In docker-compose.yml
user-service:
  deploy:
    replicas: 3  # Run 3 instances
```

### Redis Caching

ProductService caches results with 1-hour TTL:

```csharp
// Cache hit → <50ms response
// Cache miss → DB query + cache refresh
```

### Database Optimization

- Connection pooling: Max 100 connections per service
- Read replicas: Can be added for scaling reads
- Indexes on frequently queried columns

## 🚢 Deployment

### To Azure Kubernetes Service (AKS)

```bash
# Build images
docker build -t registry.azurecr.io/ecommerce/user-service:v1.0 .

# Push to registry
docker push registry.azurecr.io/ecommerce/user-service:v1.0

# Deploy with Helm charts or kubectl manifests
kubectl apply -f k8s/
```

### To Azure App Service

```bash
# Create resource group
az group create -n ecommerce -l eastus

# Deploy containers
az container create -g ecommerce -n ecommerce-app -i registry.azurecr.io/ecommerce/app:latest
```

## 📖 API Documentation

Each service exposes Swagger/OpenAPI documentation:

- **UserService**: http://localhost:5005/swagger
- **ProductService**: http://localhost:6005/swagger
- **CartService**: http://localhost:7005/swagger
- **OrderService**: http://localhost:8005/swagger
- **PaymentService**: http://localhost:5010/swagger

## 🛠️ Development

### Local Development Setup

```powershell
# Start infrastructure only (MySQL, Redis, RabbitMQ, Eureka)
cd services-ecommerce\deploy
docker-compose up eureka-server new-ecommerce-db redis rabbitmq

# In separate terminal, run individual service
cd services-ecommerce\UserService\UserService.Startup
dotnet run

# Run Angular frontend
cd ecommerce-client
npm install
npm start
```

### Running Tests

```powershell
# Backend unit tests
dotnet test services-ecommerce/

# Frontend unit tests
npm test
```

## 📦 Technologies Used

### Backend
- **ASP.NET Core 8.0** - Web framework
- **Entity Framework Core** - ORM
- **MySQL 8.0** - Database
- **Redis 7.x** - Caching
- **RabbitMQ 3.x** - Message queue
- **Ocelot** - API Gateway
- **Steeltoe** - Eureka service discovery
- **Stripe API** - Payment processing
- **SignalR** - Real-time communication
- **Polly** - Circuit breaker patterns

### Frontend
- **Angular 20.1+** - Web framework
- **TypeScript** - Language
- **RxJS** - Reactive programming
- **SignalR Client** - Real-time communication
- **Bootstrap/SCSS** - Styling

### Infrastructure
- **Docker** - Containerization
- **Docker Compose** - Orchestration
- **Eureka** - Service discovery
- **MySQL** - Primary database
- **Redis** - Cache store
- **RabbitMQ** - Message broker

## 📄 License

This project is part of NAGP Assignment.

## 🤝 Support

For questions or issues:
1. Check service logs: `docker-compose logs service-name`
2. Review Swagger documentation at service endpoints
3. Check Eureka dashboard for service health: http://localhost:8761
4. Verify database: `docker exec -it deploy-new-ecommerce-db-1 mysql -uroot -proot123`

---

**Happy Coding! 🚀**

Last Updated: March 2026

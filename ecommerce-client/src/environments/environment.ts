export const environment = {
  apiGatewayUrl: 'http://localhost:9005',
  production: false,
  apiEndpoints: {
    authServiceUrl: `http://localhost:9005/userService/api/auth`,
    userServiceUrl: 'http://localhost:9005/userService/api/user',
    productServiceUrl: 'http://localhost:9005/productService/api/products',
    cartServiceUrl: 'http://localhost:9005/cartService/api/carts',
    orderServiceUrl: 'http://localhost:9005/orderService/api/order',
    paymentServiceUrl: 'http://localhost:9005/paymentService/api/payment'
  },
  signalRUrl: 'http://localhost:8005/hubs/orderstatus',
  stripe: {
    publishableKey: 'pk_test_your_stripe_publishable_key_here'
  }
};

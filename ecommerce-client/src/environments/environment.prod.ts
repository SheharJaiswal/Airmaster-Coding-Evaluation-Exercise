export const environment = {
  production: true,
  apiUrl: 'https://your-production-api.azurewebsites.net',
  apiEndpoints: {
    auth: '/user/api/Auth',
    user: '/user/api/User',
    product: '/product/api/Product',
    cart: '/cart/api/Cart',
    order: '/order/api/Order',
    payment: '/payment/api/Payment'
  },
  signalRUrl: 'https://your-production-api.azurewebsites.net/hubs/orderstatus',
  stripe: {
    publishableKey: 'pk_live_your_stripe_publishable_key_here'
  }
};

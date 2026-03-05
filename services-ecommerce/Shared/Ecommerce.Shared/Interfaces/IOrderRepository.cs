using Ecommerce.Shared.Models;

namespace Ecommerce.Shared.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderDto> GetOrderAsync(string orderId);
        Task<OrderDto> CreateOrderAsync(string cartId);
    }
}

using Ecommerce.Shared.Models;

namespace Ecommerce.Shared.Interfaces
{
    public interface INotificationRepository
    {
        Task PublishOrderCreatedAsync(OrderDto order);
    }
}

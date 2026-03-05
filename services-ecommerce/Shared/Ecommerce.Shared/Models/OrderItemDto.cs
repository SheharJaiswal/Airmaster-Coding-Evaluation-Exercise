using Ecommerce.Shared.Enums;

namespace Ecommerce.Shared.Models
{
    public class OrderItemDto
    {
        public string OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public string ProductId { get; set; }
    }
}

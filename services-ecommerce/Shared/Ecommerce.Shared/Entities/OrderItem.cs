using Ecommerce.Shared.Enums;

namespace Ecommerce.Shared.Entities
{
    public class OrderItem
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public string ProductId {  get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}

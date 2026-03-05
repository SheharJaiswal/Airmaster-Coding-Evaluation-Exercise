namespace Ecommerce.Shared.Models
{
    public class OrderDto
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> OrderItemDtos { get; set; }
    }
}

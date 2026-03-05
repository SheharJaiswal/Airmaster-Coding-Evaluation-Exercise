namespace Ecommerce.Shared.Entities
{
    public class Cart
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CartItem> Items { get; set; } = new();
    }
}

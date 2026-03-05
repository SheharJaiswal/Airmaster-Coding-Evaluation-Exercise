namespace Ecommerce.Shared.DTOs
{
    public class CartResponseDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CartItemResponseDto> Items { get; set; } = new();
    }
}

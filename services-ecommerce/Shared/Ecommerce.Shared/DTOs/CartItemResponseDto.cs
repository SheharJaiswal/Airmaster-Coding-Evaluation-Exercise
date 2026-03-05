namespace Ecommerce.Shared.DTOs
{
    public class CartItemResponseDto
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtTimeOfAddition { get; set; }
        public DateTime AddedAt { get; set; }
    }
}

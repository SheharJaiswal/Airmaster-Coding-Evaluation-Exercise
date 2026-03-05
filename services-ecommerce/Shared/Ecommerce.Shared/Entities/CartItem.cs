namespace Ecommerce.Shared.Entities
{
    public class CartItem
    {
        public string Id { get; set; }
        public string CartId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtTimeOfAddition { get; set; }
        public DateTime AddedAt { get; set; }
        public Cart? Cart { get; set; }
    }
}

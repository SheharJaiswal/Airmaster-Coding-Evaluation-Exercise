using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Entities;

namespace Ecommerce.Shared.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<Cart> CreateCartAsync(string userId);
        Task<CartItem> AddItemToCartAsync(string cartId, string productId, int quantity, decimal price);
        Task<bool> RemoveItemFromCartAsync(string cartId, string productId);
        Task<bool> UpdateItemQuantityAsync(string cartId, string productId, int newQuantity);
        Task<bool> ClearCartAsync(string cartId);
        Task<UserDto> GetUserAsync(string userId);
        Task<ProductDto> GetProductAsync(string productId);
    }
}

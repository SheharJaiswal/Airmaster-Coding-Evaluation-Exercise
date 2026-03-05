using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Interfaces.DbContexts;
using Ecommerce.Shared.Interfaces;
using Ecommerce.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Shared.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ICartDbContext _context;

        public CartRepository(ICartDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> CreateCartAsync(string userId)
        {
            var cart = new Cart {
                UserId = userId,
                Id=Guid.NewGuid().ToString(),
                UpdatedAt = DateTime.UtcNow,
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartItem> AddItemToCartAsync(string cartId, string productId, int quantity, decimal price)
        {
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                // _context.CartItems.Update(existingItem); // tracked
            }
            else
            {
                existingItem = new CartItem
                {
                    Id=Guid.NewGuid().ToString(),
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity,
                    PriceAtTimeOfAddition = price,
                    AddedAt= DateTime.UtcNow
                };
                _context.CartItems.Add(existingItem);
            }

            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task<bool> RemoveItemFromCartAsync(string cartId, string productId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId);

            if (item == null) return false;

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateItemQuantityAsync(string cartId, string productId, int newQuantity)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId);

            if (item == null) return false;

            item.Quantity = newQuantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(string cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null) return false;

            _context.CartItems.RemoveRange(cart.Items);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDto> GetUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return new UserDto()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
            };
        }

        public async Task<ProductDto> GetProductAsync(string productId)
        {
            var product = await _context.Products.FindAsync(productId);
            return new ProductDto()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };
        }
    }
}

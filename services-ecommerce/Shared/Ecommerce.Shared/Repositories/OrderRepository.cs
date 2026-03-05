using Microsoft.EntityFrameworkCore;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Interfaces.DbContexts;
using Ecommerce.Shared.Interfaces;
using Ecommerce.Shared.Models;

namespace Ecommerce.Shared.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderDbContext _context;

        public OrderRepository(IOrderDbContext orderDbContext)
        {
            _context = orderDbContext;
        }
        public async Task<OrderDto> CreateOrderAsync(string cartId)
        {
            var cart = await _context.Carts.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == cartId);
            if (cart == null)
            {
                throw new Exception("Cart is empty");
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == cart.UserId);
            string orderId = Guid.NewGuid().ToString();
            var newOrder = new Order()
            {
                Id = orderId,
                CreatedAt = DateTime.Now,
                CreatedBy = user.Username,
            };

            foreach (var item in cart.Items)
            {
                newOrder.OrderItems.Add(new()
                {
                    Id = item.Id,
                    Status = Ecommerce.Shared.Enums.OrderStatus.Created,
                    ProductId = item.ProductId,
                    OrderId = orderId,
                });
            }

            _ = await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();
            await RemoveCartItems(cartId);

            return new OrderDto()
            {
                CreatedAt = newOrder.CreatedAt,
                CreatedBy = newOrder.CreatedBy,
                OrderItemDtos = newOrder.OrderItems.Select(x => new OrderItemDto()
                {
                    OrderId = x.Id,
                    Status = x.Status,
                    ProductId = x.ProductId,
                }).ToList(),
            };
        }

        private async Task RemoveCartItems(string cartId)
        {
            var cartItems = await _context.CartItems.Where(x => x.CartId == cartId).ToListAsync();
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<OrderDto> GetOrderAsync(string orderId)
        {
            var order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == orderId);
            return new OrderDto()
            {
                CreatedAt = order.CreatedAt,
                CreatedBy = order.CreatedBy,
                OrderItemDtos = order.OrderItems.Select(x => new OrderItemDto()
                {
                    OrderId = x.Id,
                    Status = x.Status,
                    ProductId = x.ProductId,
                }).ToList(),
            };
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Ecommerce.Shared.Entities;

namespace Ecommerce.Shared.Interfaces.DbContexts
{
    public interface IOrderDbContext
    {
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<Cart> Carts { get; }
        DbSet<CartItem> CartItems { get; }
        DbSet<Product> Products { get; }
        DbSet<User> Users { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

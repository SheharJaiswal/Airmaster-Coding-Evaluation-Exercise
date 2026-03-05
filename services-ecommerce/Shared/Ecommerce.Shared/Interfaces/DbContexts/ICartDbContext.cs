using Microsoft.EntityFrameworkCore;
using Ecommerce.Shared.Entities;

namespace Ecommerce.Shared.Interfaces.DbContexts
{
    public interface ICartDbContext
    {
        DbSet<Cart> Carts { get; }
        DbSet<CartItem> CartItems { get; }
        DbSet<User> Users { get; }
        DbSet<Product> Products { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

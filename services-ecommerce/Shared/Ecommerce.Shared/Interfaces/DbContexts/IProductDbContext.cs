using Microsoft.EntityFrameworkCore;
using Ecommerce.Shared.Entities;

namespace Ecommerce.Shared.Interfaces.DbContexts
{
    public interface IProductDbContext
    {
        DbSet<Product> Products { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

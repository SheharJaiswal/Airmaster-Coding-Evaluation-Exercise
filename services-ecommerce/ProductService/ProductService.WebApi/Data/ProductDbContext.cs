using Microsoft.EntityFrameworkCore;
using Ecommerce.Shared.Entities;

namespace ProductService.WebApi.Data
{
    public class ProductDbContext : DbContext, Ecommerce.Shared.Interfaces.DbContexts.IProductDbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
        : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Premium Laptop",
                    Description = "16GB RAM, 1TB SSD, Intel i7",
                    Price = 1299.99m,
                    Category = "Electronics",
                    StockQuantity = 50,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seed",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "Seed"
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Wireless Mouse",
                    Description = "Ergonomic design, 2.4GHz wireless",
                    Price = 29.99m,
                    Category = "Accessories",
                    StockQuantity = 200,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seed",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "Seed"
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Mechanical Keyboard",
                    Description = "RGB backlight, Cherry MX switches",
                    Price = 89.99m,
                    Category = "Accessories",
                    StockQuantity = 75,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seed",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = "Seed"
                }
            );
        }
    }
}
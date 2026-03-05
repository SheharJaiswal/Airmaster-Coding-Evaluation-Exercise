using Microsoft.EntityFrameworkCore;
using Ecommerce.Shared.Entities;
using Humanizer;

namespace OrderService.WebApi.Data
{
    public class OrderDbContext : DbContext, Ecommerce.Shared.Interfaces.DbContexts.IOrderDbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            _ = modelBuilder.Entity<Cart>().ToTable(name: nameof(Cart).Pluralize(), t => t.ExcludeFromMigrations());
            _ = modelBuilder.Entity<CartItem>().ToTable(name: nameof(CartItem).Pluralize(), t => t.ExcludeFromMigrations());
            _ = modelBuilder.Entity<Product>().ToTable(name: nameof(Product).Pluralize(), t => t.ExcludeFromMigrations());
            _ = modelBuilder.Entity<User>().ToTable(name: nameof(User).Pluralize(), t => t.ExcludeFromMigrations());
        }
    }
}
using Ecommerce.Shared.Entities;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace CartService.WebApi.Data
{
    public class CartDbContext : DbContext, Ecommerce.Shared.Interfaces.DbContexts.ICartDbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
        {
        }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Cart -> CartItems relationship
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mark User and Product tables as external (not managed by CartService migrations)
            _ = modelBuilder.Entity<User>().ToTable(name: nameof(User).Pluralize(), t => t.ExcludeFromMigrations());
            _ = modelBuilder.Entity<Product>().ToTable(name: nameof(Product).Pluralize(), t => t.ExcludeFromMigrations());

            // Remove the foreign key to Users since User table is managed by UserService
            // We keep UserId as a string, but don't enforce foreign key constraint
        }
    }
}
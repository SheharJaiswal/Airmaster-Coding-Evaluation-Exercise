using Microsoft.EntityFrameworkCore;
using Ecommerce.Shared.Entities;

namespace UserService.WebApi.Data
{
    public class UserDbContext : DbContext, Ecommerce.Shared.Interfaces.DbContexts.IUserDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Seed initial admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
using Microsoft.EntityFrameworkCore;

namespace PaymentService.WebApi.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderId).IsRequired();
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.Status).IsRequired();
                entity.HasIndex(e => e.OrderId);
            });

            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.PaymentId);
            });
        }
    }

    public class Payment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OrderId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed, Refunded
        public string? StripePaymentIntentId { get; set; }
        public string? PayPalOrderId { get; set; }
        public string PaymentMethod { get; set; } = "Stripe"; // Stripe, PayPal
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PaymentTransaction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PaymentId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Charge, Refund, Capture
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Metadata { get; set; }
    }
}

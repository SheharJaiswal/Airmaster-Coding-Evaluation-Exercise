using PaymentService.WebApi.Data;
using Stripe;
using Polly;
using Polly.CircuitBreaker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace PaymentService.WebApi.Services
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
        Task<RefundResult> RefundPaymentAsync(string paymentId);
        Task<Payment?> GetPaymentByOrderIdAsync(string orderId);
    }

    public class StripePaymentService : IPaymentService
    {
        private readonly PaymentDbContext _dbContext;
        private readonly ILogger<StripePaymentService> _logger;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public StripePaymentService(
            PaymentDbContext dbContext, 
            ILogger<StripePaymentService> logger,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _logger = logger;
            
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"] ?? "sk_test_placeholder";

            // Circuit breaker: Open after 3 consecutive failures, stay open for 30 seconds
            _circuitBreakerPolicy = Policy
                .Handle<StripeException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, duration) =>
                    {
                        _logger.LogWarning($"Circuit breaker opened for {duration.TotalSeconds}s due to: {exception.Message}");
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Circuit breaker reset");
                    }
                );
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            var payment = new Payment
            {
                OrderId = request.OrderId,
                UserId = request.UserId,
                Amount = request.Amount,
                Currency = request.Currency,
                PaymentMethod = "Stripe",
                Status = "Pending"
            };

            try
            {
                // Execute with circuit breaker
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = (long)(request.Amount * 100), // Convert to cents
                        Currency = request.Currency.ToLower(),
                        PaymentMethod = request.PaymentMethodId,
                        Confirm = true,
                        AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                        {
                            Enabled = true,
                            AllowRedirects = "never"
                        },
                        Metadata = new Dictionary<string, string>
                        {
                            { "order_id", request.OrderId },
                            { "user_id", request.UserId }
                        }
                    };

                    var service = new PaymentIntentService();
                    var paymentIntent = await service.CreateAsync(options);

                    payment.StripePaymentIntentId = paymentIntent.Id;
                    payment.Status = paymentIntent.Status == "succeeded" ? "Success" : "Failed";
                    payment.UpdatedAt = DateTime.UtcNow;
                });

                await _dbContext.Payments.AddAsync(payment);
                await _dbContext.SaveChangesAsync();

                return new PaymentResult
                {
                    Success = payment.Status == "Success",
                    PaymentId = payment.Id,
                    TransactionId = payment.StripePaymentIntentId,
                    Message = payment.Status == "Success" ? "Payment processed successfully" : "Payment failed"
                };
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, "Circuit breaker is open - payment service unavailable");
                payment.Status = "Failed";
                payment.ErrorMessage = "Payment service temporarily unavailable";
                await _dbContext.Payments.AddAsync(payment);
                await _dbContext.SaveChangesAsync();

                return new PaymentResult
                {
                    Success = false,
                    PaymentId = payment.Id,
                    Message = "Payment service temporarily unavailable. Please try again later."
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe payment failed");
                payment.Status = "Failed";
                payment.ErrorMessage = ex.Message;
                await _dbContext.Payments.AddAsync(payment);
                await _dbContext.SaveChangesAsync();

                return new PaymentResult
                {
                    Success = false,
                    PaymentId = payment.Id,
                    Message = ex.Message
                };
            }
        }

        public async Task<RefundResult> RefundPaymentAsync(string paymentId)
        {
            var payment = await _dbContext.Payments.FindAsync(paymentId);
            if (payment == null)
            {
                return new RefundResult { Success = false, Message = "Payment not found" };
            }

            try
            {
                if (payment.StripePaymentIntentId == null)
                {
                    return new RefundResult { Success = false, Message = "Invalid payment" };
                }

                var refundService = new RefundService();
                var refund = await refundService.CreateAsync(new RefundCreateOptions
                {
                    PaymentIntent = payment.StripePaymentIntentId
                });

                payment.Status = "Refunded";
                payment.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return new RefundResult
                {
                    Success = true,
                    RefundId = refund.Id,
                    Message = "Refund processed successfully"
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Refund failed");
                return new RefundResult { Success = false, Message = ex.Message };
            }
        }

        public async Task<Payment?> GetPaymentByOrderIdAsync(string orderId)
        {
            return await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }
    }

    public class PaymentRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string PaymentMethodId { get; set; } = string.Empty;
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class RefundResult
    {
        public bool Success { get; set; }
        public string? RefundId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

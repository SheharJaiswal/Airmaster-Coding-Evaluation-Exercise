using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.WebApi.Services;

namespace PaymentService.Startup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("process")]
        [Authorize]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                request.UserId = userId;
                var result = await _paymentService.ProcessPaymentAsync(request);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment processing failed");
                return StatusCode(500, new { message = "Payment processing failed", error = ex.Message });
            }
        }

        [HttpPost("refund/{paymentId}")]
        [Authorize]
        public async Task<IActionResult> RefundPayment(string paymentId)
        {
            try
            {
                var result = await _paymentService.RefundPaymentAsync(paymentId);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refund processing failed");
                return StatusCode(500, new { message = "Refund failed", error = ex.Message });
            }
        }

        [HttpGet("order/{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentByOrderId(string orderId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
                
                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found" });
                }
                
                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve payment");
                return StatusCode(500, new { message = "Failed to retrieve payment", error = ex.Message });
            }
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "PaymentService", timestamp = DateTime.UtcNow });
        }
    }
}

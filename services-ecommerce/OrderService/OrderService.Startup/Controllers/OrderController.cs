using Ecommerce.Shared.Interfaces;
using Ecommerce.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Startup.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShippingService _shippingService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderRepository orderRepository,
            IShippingService shippingService,
            ILogger<OrderController> logger)
        {
            _orderRepository = orderRepository;
            _shippingService = shippingService;
            _logger = logger;
        }

        [HttpPost("{cartId}")]
        public async Task<IActionResult> CreateOrder(string cartId)
        {
            try
            {
                var order = await _orderRepository.CreateOrderAsync(cartId);
                
                // Create shipment automatically when order is created
                try
                {
                    var shippingRequest = new ShippingRequest
                    {
                        OrderId = cartId, // Use cartId as reference
                        Carrier = "FedEx", // Default carrier
                        ToAddress = new Address
                        {
                            Street = "Customer Address", // In production, get from user profile
                            City = "New York",
                            State = "NY",
                            ZipCode = "10001",
                            Country = "USA"
                        },
                        FromAddress = new Address
                        {
                            Street = "456 Warehouse Blvd",
                            City = "Los Angeles",
                            State = "CA",
                            ZipCode = "90001",
                            Country = "USA"
                        },
                        Package = new Package
                        {
                            Weight = 5.0, // Default weight in lbs
                            Length = 12,
                            Width = 10,
                            Height = 8
                        }
                    };

                    var shipmentResult = await _shippingService.CreateShipmentAsync(shippingRequest);
                    
                    if (shipmentResult.Success)
                    {
                        _logger.LogInformation($"Created shipment for order/cart {cartId}: {shipmentResult.TrackingNumber}");
                        
                        return Ok(new
                        {
                            order,
                            shipping = new
                            {
                                trackingNumber = shipmentResult.TrackingNumber,
                                carrier = shipmentResult.Carrier,
                                estimatedDelivery = shipmentResult.EstimatedDelivery,
                                labelUrl = shipmentResult.Label?.LabelUrl
                            }
                        });
                    }
                }
                catch (Exception shipEx)
                {
                    _logger.LogError(shipEx, $"Failed to create shipment for order {cartId}");
                    // Continue even if shipping fails - order is still created
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(string orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderAsync(orderId);
                if (order == null)
                    return NotFound();

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch order");
                return StatusCode(500, new { message = "An error occurred while fetching the order." });
            }
        }

        [HttpGet("tracking/{trackingNumber}")]
        public async Task<IActionResult> GetTrackingInfo(string trackingNumber, [FromQuery] string carrier = "FedEx")
        {
            try
            {
                var trackingInfo = await _shippingService.GetTrackingInfoAsync(trackingNumber, carrier);
                return Ok(trackingInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get tracking info for {trackingNumber}");
                return StatusCode(500, new { message = "Failed to retrieve tracking information." });
            }
        }

        [HttpDelete("shipment/{trackingNumber}")]
        public async Task<IActionResult> CancelShipment(string trackingNumber)
        {
            try
            {
                var result = await _shippingService.CancelShipmentAsync(trackingNumber);
                if (result)
                    return Ok(new { message = "Shipment cancelled successfully" });
                
                return NotFound(new { message = "Shipment not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to cancel shipment {trackingNumber}");
                return StatusCode(500, new { message = "Failed to cancel shipment." });
            }
        }
    }
}

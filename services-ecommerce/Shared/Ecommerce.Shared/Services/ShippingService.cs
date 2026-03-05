using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Services
{
    // Prototype Shipping Service Interface
    public interface IShippingService
    {
        Task<ShippingResult> CreateShipmentAsync(ShippingRequest request);
        Task<TrackingInfo> GetTrackingInfoAsync(string trackingNumber, string carrier);
        Task<bool> CancelShipmentAsync(string trackingNumber);
    }

    // Mock implementation for prototype
    public class MockShippingService : IShippingService
    {
        private readonly ILogger<MockShippingService> _logger;
        private readonly Dictionary<string, ShipmentData> _mockShipments = new();

        public MockShippingService(ILogger<MockShippingService> logger)
        {
            _logger = logger;
        }

        public async Task<ShippingResult> CreateShipmentAsync(ShippingRequest request)
        {
            // Mock FedEx/UPS API call
            await Task.Delay(500); // Simulate API latency

            var trackingNumber = $"{request.Carrier.ToUpper()}{DateTime.UtcNow.Ticks}";
            
            _mockShipments[trackingNumber] = new ShipmentData
            {
                TrackingNumber = trackingNumber,
                Carrier = request.Carrier,
                OrderId = request.OrderId,
                Status = "Created",
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation($"Created shipment {trackingNumber} for order {request.OrderId}");

            return new ShippingResult
            {
                Success = true,
                TrackingNumber = trackingNumber,
                Carrier = request.Carrier,
                EstimatedDelivery = DateTime.UtcNow.AddDays(3),
                Label = new ShippingLabel
                {
                    LabelUrl = $"https://mockshipper.com/labels/{trackingNumber}.pdf",
                    Format = "PDF"
                }
            };
        }

        public async Task<TrackingInfo> GetTrackingInfoAsync(string trackingNumber, string carrier)
        {
            await Task.Delay(200);

            if (_mockShipments.TryGetValue(trackingNumber, out var shipment))
            {
                return new TrackingInfo
                {
                    TrackingNumber = trackingNumber,
                    Carrier = carrier,
                    Status = "In Transit",
                    EstimatedDelivery = DateTime.UtcNow.AddDays(2),
                    Events = new List<TrackingEvent>
                    {
                        new() { Status = "Created", Location = "Origin Facility", Timestamp = shipment.CreatedAt },
                        new() { Status = "In Transit", Location = "Sort Facility", Timestamp = DateTime.UtcNow }
                    }
                };
            }

            return new TrackingInfo
            {
                TrackingNumber = trackingNumber,
                Carrier = carrier,
                Status = "Not Found"
            };
        }

        public async Task<bool> CancelShipmentAsync(string trackingNumber)
        {
            await Task.Delay(200);
            
            if (_mockShipments.Remove(trackingNumber))
            {
                _logger.LogInformation($"Cancelled shipment {trackingNumber}");
                return true;
            }

            return false;
        }
    }

    // Models
    public class ShippingRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public string Carrier { get; set; } = "FedEx"; // FedEx, UPS, USPS
        public Address FromAddress { get; set; } = new();
        public Address ToAddress { get; set; } = new();
        public Package Package { get; set; } = new();
    }

    public class Address
    {
        public string Name { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = "US";
        public string Phone { get; set; } = string.Empty;
    }

    public class Package
    {
        public double Weight { get; set; } // in pounds
        public double Length { get; set; } // in inches
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class ShippingResult
    {
        public bool Success { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string Carrier { get; set; } = string.Empty;
        public DateTime EstimatedDelivery { get; set; }
        public ShippingLabel? Label { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ShippingLabel
    {
        public string LabelUrl { get; set; } = string.Empty;
        public string Format { get; set; } = "PDF";
    }

    public class TrackingInfo
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public string Carrier { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? EstimatedDelivery { get; set; }
        public List<TrackingEvent> Events { get; set; } = new();
    }

    public class TrackingEvent
    {
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    internal class ShipmentData
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public string Carrier { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

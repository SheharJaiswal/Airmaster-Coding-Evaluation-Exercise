using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace OrderService.Startup.Hubs
{
    [Authorize]
    public class OrderStatusHub : Hub
    {
        private readonly ILogger<OrderStatusHub> _logger;

        public OrderStatusHub(ILogger<OrderStatusHub> logger)
        {
            _logger = logger;
        }

        public async Task SubscribeToOrder(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");
            _logger.LogInformation($"Client {Context.ConnectionId} subscribed to order {orderId}");
        }

        public async Task UnsubscribeFromOrder(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order-{orderId}");
            _logger.LogInformation($"Client {Context.ConnectionId} unsubscribed from order {orderId}");
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
                _logger.LogInformation($"User {userId} connected with connection ID {Context.ConnectionId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client {Context.ConnectionId} disconnected");
            await base.OnDisconnectedAsync(exception);
        }
    }

    // Service to send notifications
    public interface IOrderNotificationService
    {
        Task NotifyOrderStatusChange(string orderId, string status, string message);
        Task NotifyUserOrders(string userId, string message);
    }

    public class OrderNotificationService : IOrderNotificationService
    {
        private readonly IHubContext<OrderStatusHub> _hubContext;
        private readonly ILogger<OrderNotificationService> _logger;

        public OrderNotificationService(
            IHubContext<OrderStatusHub> hubContext,
            ILogger<OrderNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyOrderStatusChange(string orderId, string status, string message)
        {
            try
            {
                await _hubContext.Clients
                    .Group($"order-{orderId}")
                    .SendAsync("OrderStatusChanged", new
                    {
                        orderId,
                        status,
                        message,
                        timestamp = DateTime.UtcNow
                    });

                _logger.LogInformation($"Notified order {orderId} status change: {status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send notification for order {orderId}");
            }
        }

        public async Task NotifyUserOrders(string userId, string message)
        {
            try
            {
                await _hubContext.Clients
                    .Group($"user-{userId}")
                    .SendAsync("UserNotification", new
                    {
                        message,
                        timestamp = DateTime.UtcNow
                    });

                _logger.LogInformation($"Notified user {userId}: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send notification to user {userId}");
            }
        }
    }
}

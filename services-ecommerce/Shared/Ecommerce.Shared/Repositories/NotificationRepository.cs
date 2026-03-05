using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Ecommerce.Shared.Interfaces;
using Ecommerce.Shared.Models;

namespace Ecommerce.Shared.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IConnection _connection;
        private readonly string _queueName;

        public NotificationRepository(IConnection connection, IConfiguration configuration)
        {
            _connection = connection;
            _queueName = configuration["RabbitMQ:QueueName"] ?? "order.created";
        }

        public Task PublishOrderCreatedAsync(OrderDto order)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(order));

            channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: null,
                body: body
            );

            return Task.CompletedTask;
        }
    }
}

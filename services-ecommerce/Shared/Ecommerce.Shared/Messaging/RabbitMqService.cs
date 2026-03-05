using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Polly;
using Polly.CircuitBreaker;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Shared.Messaging
{
    public interface IMessageQueueService
    {
        Task PublishAsync<T>(string queueName, T message);
        Task<bool> ConsumeAsync<T>(string queueName, Func<T, Task<bool>> messageHandler);
    }

    public class RabbitMqService : IMessageQueueService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMqService> _logger;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public RabbitMqService(IConnectionFactory connectionFactory, ILogger<RabbitMqService> logger)
        {
            _logger = logger;
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            // Circuit breaker for RabbitMQ operations
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(60),
                    onBreak: (exception, duration) =>
                    {
                        _logger.LogWarning($"RabbitMQ circuit breaker opened for {duration.TotalSeconds}s: {exception.Message}");
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("RabbitMQ circuit breaker reset");
                    }
                );
        }

        public async Task PublishAsync<T>(string queueName, T message)
        {
            try
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    // Declare main queue
                    _channel.QueueDeclare(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: new Dictionary<string, object>
                        {
                            { "x-dead-letter-exchange", $"{queueName}-dlx" },
                            { "x-dead-letter-routing-key", $"{queueName}-dlq" }
                        });

                    // Declare dead-letter exchange
                    _channel.ExchangeDeclare($"{queueName}-dlx", ExchangeType.Direct, durable: true);

                    // Declare dead-letter queue
                    _channel.QueueDeclare(
                        queue: $"{queueName}-dlq",
                        durable: true,
                        exclusive: false,
                        autoDelete: false);

                    // Bind dead-letter queue to exchange
                    _channel.QueueBind($"{queueName}-dlq", $"{queueName}-dlx", $"{queueName}-dlq");

                    var json = JsonSerializer.Serialize(message);
                    var body = Encoding.UTF8.GetBytes(json);

                    var properties = _channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.DeliveryMode = 2;
                    properties.Headers = new Dictionary<string, object>
                    {
                        { "x-max-retries", 3 },
                        { "x-retry-count", 0 }
                    };

                    _channel.BasicPublish(
                        exchange: "",
                        routingKey: queueName,
                        basicProperties: properties,
                        body: body);

                    _logger.LogInformation($"Published message to queue {queueName}");
                    await Task.CompletedTask;
                });
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, "RabbitMQ circuit is open - message not published");
                throw new InvalidOperationException("Message queue temporarily unavailable", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to publish message to queue {queueName}");
                throw;
            }
        }

        public async Task<bool> ConsumeAsync<T>(string queueName, Func<T, Task<bool>> messageHandler)
        {
            try
            {
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", $"{queueName}-dlx" },
                        { "x-dead-letter-routing-key", $"{queueName}-dlq" }
                    });

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    try
                    {
                        var message = JsonSerializer.Deserialize<T>(json);
                        if (message != null)
                        {
                            var success = await messageHandler(message);

                            if (success)
                            {
                                _channel.BasicAck(ea.DeliveryTag, false);
                                _logger.LogInformation($"Successfully processed message from {queueName}");
                            }
                            else
                            {
                                HandleRetry(ea, queueName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing message from {queueName}");
                        HandleRetry(ea, queueName);
                    }
                };

                _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                _logger.LogInformation($"Started consuming from queue {queueName}");

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to start consuming from queue {queueName}");
                return false;
            }
        }

        private void HandleRetry(BasicDeliverEventArgs ea, string queueName)
        {
            var headers = ea.BasicProperties.Headers ?? new Dictionary<string, object>();
            var maxRetries = headers.ContainsKey("x-max-retries") ? Convert.ToInt32(headers["x-max-retries"]) : 3;
            var retryCount = headers.ContainsKey("x-retry-count") ? Convert.ToInt32(headers["x-retry-count"]) : 0;

            if (retryCount < maxRetries)
            {
                // Increment retry count and requeue
                headers["x-retry-count"] = retryCount + 1;
                _channel.BasicNack(ea.DeliveryTag, false, true);
                _logger.LogWarning($"Message requeued (retry {retryCount + 1}/{maxRetries}) in {queueName}");
            }
            else
            {
                // Move to dead-letter queue
                _channel.BasicNack(ea.DeliveryTag, false, false);
                _logger.LogError($"Message moved to dead-letter queue after {maxRetries} retries in {queueName}");
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }

    // Message Models
    public class OrderCreatedEvent
    {
        public string OrderId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentProcessedEvent
    {
        public string OrderId { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
    }

    public class OrderShippedEvent
    {
        public string OrderId { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public string Carrier { get; set; } = string.Empty;
        public DateTime ShippedAt { get; set; }
    }
}

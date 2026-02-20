using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Soa.ChatService.Services;

public class RabbitMqPublisher : IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly ILogger<RabbitMqPublisher> _logger;

    private RabbitMqPublisher(IConnection connection, IChannel channel, ILogger<RabbitMqPublisher> logger)
    {
        _connection = connection;
        _channel = channel;
        _logger = logger;
    }

    public static async Task<RabbitMqPublisher> CreateAsync(IConfiguration configuration, ILogger<RabbitMqPublisher> logger)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"] ?? "rabbitmq",
            UserName = configuration["RabbitMq:UserName"] ?? "guest",
            Password = configuration["RabbitMq:Password"] ?? "guest"
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: "chat_messages", durable: false, exclusive: false, autoDelete: false);

        return new RabbitMqPublisher(connection, channel, logger);
    }

    public async Task PublishChatMessageAsync(string adId, string userId, string message)
    {
        try
        {
            var payload = JsonSerializer.Serialize(new
            {
                pattern = "chat.newMessage",
                data = new { adId, userId, message, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }
            });

            var body = Encoding.UTF8.GetBytes(payload);
            await _channel.BasicPublishAsync(exchange: "", routingKey: "chat_messages", body: body);
            _logger.LogInformation("RabbitMQ message sent for ad {AdId}", adId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish RabbitMQ message for ad {AdId}", adId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
    }
}

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Soa.Gateway.Hubs;

namespace Soa.Gateway.Services;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly IHubContext<ChatHub> _chatHub;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqConsumerService> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumerService(
        IHubContext<ChatHub> chatHub,
        IConfiguration configuration,
        ILogger<RabbitMqConsumerService> logger)
    {
        _chatHub = chatHub;
        _configuration = configuration;
        _logger = logger;
    }

    private const int StartupDelaySeconds = 10;
    private const int RetryDelaySeconds = 10;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() => _logger.LogInformation("RabbitMQ consumer stopping"));

        await Task.Delay(TimeSpan.FromSeconds(StartupDelaySeconds), stoppingToken);

        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:HostName"] ?? "rabbitmq",
            UserName = _configuration["RabbitMq:UserName"] ?? "guest",
            Password = _configuration["RabbitMq:Password"] ?? "guest"
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CloseConnectionAsync();

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.QueueDeclareAsync(queue: "chat_messages", durable: false, exclusive: false, autoDelete: false);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (_, ea) =>
                {
                    try
                    {
                        var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                        using var doc = JsonDocument.Parse(body);
                        var root = doc.RootElement;

                        if (root.TryGetProperty("data", out var data))
                        {
                            var adId = data.GetProperty("adId").GetString() ?? "";
                            await _chatHub.Clients.Group(adId).SendAsync("NewMessage", data, stoppingToken);
                            _logger.LogInformation("Forwarded chat message for ad {AdId}", adId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing RabbitMQ message");
                    }
                };

                await _channel.BasicConsumeAsync(queue: "chat_messages", autoAck: true, consumer: consumer);
                _logger.LogInformation("RabbitMQ consumer started on queue chat_messages");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RabbitMQ connection failed, retrying in {Seconds}s...", RetryDelaySeconds);
                await Task.Delay(TimeSpan.FromSeconds(RetryDelaySeconds), stoppingToken);
            }
        }
    }

    private async Task CloseConnectionAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
            _channel = null;
        }
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
            _connection = null;
        }
    }

    public override void Dispose()
    {
        CloseConnectionAsync().GetAwaiter().GetResult();
        base.Dispose();
    }
}

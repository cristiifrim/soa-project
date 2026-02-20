using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Soa.Gateway.Hubs;

namespace Soa.Gateway.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly IHubContext<NotificationsHub> _notificationsHub;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaConsumerService> _logger;

    private const int StartupDelaySeconds = 15;
    private const int RetryDelaySeconds = 10;

    public KafkaConsumerService(
        IHubContext<NotificationsHub> notificationsHub,
        IConfiguration configuration,
        ILogger<KafkaConsumerService> logger)
    {
        _notificationsHub = notificationsHub;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(StartupDelaySeconds), stoppingToken);

        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "kafka:9093",
            GroupId = "dotnet-gateway-consumer",
            AutoOffsetReset = AutoOffsetReset.Latest
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            IConsumer<Ignore, string>? consumer = null;
            try
            {
                consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe("ad-notifications");
                _logger.LogInformation("Kafka consumer connected on topic ad-notifications");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(stoppingToken);
                        if (result?.Message?.Value is null) continue;

                        var notification = JsonSerializer.Deserialize<JsonElement>(result.Message.Value);
                        await _notificationsHub.Clients.All.SendAsync("NewNotification", notification, stoppingToken);
                        _logger.LogInformation("Forwarded Kafka notification to SignalR clients");
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error consuming Kafka message");
                        await Task.Delay(1000, stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Kafka connection failed, retrying in {Seconds}s...", RetryDelaySeconds);
                consumer?.Dispose();
                await Task.Delay(TimeSpan.FromSeconds(RetryDelaySeconds), stoppingToken);
            }
            finally
            {
                try
                {
                    consumer?.Close();
                }
                catch
                {
                    // ignore on shutdown
                }
                consumer?.Dispose();
            }
        }
    }
}

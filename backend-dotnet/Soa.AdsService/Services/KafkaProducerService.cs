using System.Text.Json;
using Confluent.Kafka;

namespace Soa.AdsService.Services;

public class KafkaProducerService : IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;

    public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "kafka:9093"
        };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAdNotificationAsync(string adId, string title)
    {
        try
        {
            var payload = JsonSerializer.Serialize(new
            {
                adId,
                title,
                createdAt = DateTime.UtcNow
            });

            await _producer.ProduceAsync("ad-notifications", new Message<Null, string> { Value = payload });
            _logger.LogInformation("Kafka message sent for ad {AdId}", adId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send Kafka message for ad {AdId}", adId);
        }
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}

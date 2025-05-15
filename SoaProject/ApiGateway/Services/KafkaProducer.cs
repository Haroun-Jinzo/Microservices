using Confluent.Kafka;
using Microsoft.Extensions.Logging;

public interface IKafkaProducer
{
    Task<bool> ProduceAsync(string topic, string message);
}

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(string bootstrapServers, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            MessageSendMaxRetries = 3,
            Acks = Acks.Leader,
            EnableDeliveryReports = true,
            MessageTimeoutMs = 5000
        };

        _producer = new ProducerBuilder<Null, string>(config)
            .SetLogHandler((_, logMessage) => 
                _logger.Log(GetLogLevel(logMessage.Level), logMessage.Message))
            .Build();
    }

    public async Task<bool> ProduceAsync(string topic, string message)
    {
        try
        {
            var deliveryResult = await _producer.ProduceAsync(
                topic, 
                new Message<Null, string> { Value = message }
            );

            _logger.LogInformation($"Delivered to {deliveryResult.TopicPartitionOffset}");
            return true;
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError($"Delivery failed: {e.Error.Reason}");
            return false;
        }
    }

    private static LogLevel GetLogLevel(SyslogLevel level) => level switch
    {
        SyslogLevel.Emergency or SyslogLevel.Alert or SyslogLevel.Critical or SyslogLevel.Error 
            => LogLevel.Error,
        SyslogLevel.Warning => LogLevel.Warning,
        SyslogLevel.Notice or SyslogLevel.Info => LogLevel.Information,
        SyslogLevel.Debug => LogLevel.Debug,
        _ => LogLevel.None
    };

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
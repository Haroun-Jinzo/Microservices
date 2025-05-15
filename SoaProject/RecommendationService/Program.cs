using System.Text.Json;
using Confluent.Kafka;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<KafkaConsumerService>();

var mongoClient = new MongoClient(builder.Configuration["MongoDB:ConnectionString"]);
var database = mongoClient.GetDatabase("recommendation_db");
builder.Services.AddSingleton(database);

var host = builder.Build();
host.Run();

public class KafkaConsumerService : BackgroundService
{
    private readonly IMongoCollection<Recommendation> _recommendations;
    private readonly IConsumer<Ignore, string> _consumer;

    public KafkaConsumerService(IMongoDatabase db)
    {
        _recommendations = db.GetCollection<Recommendation>("recommendations");
        
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "recommendation-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            AllowAutoCreateTopics = true,
            EnableAutoCommit = false,
            EnablePartitionEof = true,
            MaxPollIntervalMs = 300000, // 5 minutes
            SessionTimeoutMs = 10000
        };
        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for Kafka to be ready
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        
        _consumer.Subscribe("user-interactions");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = _consumer.Consume(stoppingToken);
                // Process message
            }
            catch (ConsumeException e) when (e.Error.Code == ErrorCode.UnknownTopicOrPart)
            {
                await Task.Delay(5000, stoppingToken);
                _consumer.Subscribe("user-interactions");
            }
        }
    }
}

public record InteractionEvent(string UserId, string ProductId);
public class Recommendation
{
    public string UserId { get; set; } = null!;
    public List<string> RecentProducts { get; set; } = new();
}
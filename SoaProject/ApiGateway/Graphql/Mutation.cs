using System.Text.Json;
using Confluent.Kafka;
using HotChocolate;

public class Mutation
{
    private readonly ILogger<Mutation> _logger;

    public Mutation(ILogger<Mutation> logger)
    {
        _logger = logger;
    }

    [GraphQLName("logInteraction")]
    public async Task<bool> LogInteraction(
        [Service] IKafkaProducer producer,
        string userId,
        string productId)
    {
        try
        {
            var topic = "kafka";
            var message = JsonSerializer.Serialize(new { UserId = userId, ProductId = productId });
            await producer.ProduceAsync(topic, message);
            return true;
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError("Kafka error: {Reason}", e.Error.Reason);
            throw new GraphQLException("Event logging failed");
        }
    }
}
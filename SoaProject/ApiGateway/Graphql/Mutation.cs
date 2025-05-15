using System.Text.Json;
using Confluent.Kafka;
using HotChocolate;

public class Mutation
{
    public async Task<bool> LogInteraction(
        [Service] IKafkaProducer producer,
        string userId,
        string productId)
    {
        // Proper async/await with error handling
        try
        {
            await producer.ProduceAsync("user-interactions", 
                JsonSerializer.Serialize(new { userId, productId }));
            return true;
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Kafka delivery failed: {e.Error.Reason}");
            return false;
        }
    }
}
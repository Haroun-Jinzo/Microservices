using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonRequired]
    public string Name { get; set; } = null!;
    [BsonRequired]
    public string Category { get; set; } = null!;
    public string? Description { get; internal set; }

    public double Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BakeryMongoApp.Models;

public class CustomerOrder
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string CustomerId { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}

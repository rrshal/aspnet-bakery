using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BakeryMongoApp.Models;

public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
}

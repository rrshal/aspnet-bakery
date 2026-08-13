using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BakeryMongoApp.Models;

public class CustomerOrderItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string CustomerOrderId { get; set; } = string.Empty;
    public string MenuItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

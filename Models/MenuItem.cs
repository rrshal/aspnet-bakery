using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BakeryMongoApp.Models;

public class MenuItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int ItemType { get; set; }
    public bool? HighSugar { get; set; }
    public string? Size { get; set; }
    public bool? IsHot { get; set; }
    public int? Layers { get; set; }
}

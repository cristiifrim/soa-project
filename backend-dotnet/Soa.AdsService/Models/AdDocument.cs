using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Soa.AdsService.Models;

public class AdDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("_id")]
    public string Id { get; set; } = null!;

    [BsonElement("title")]
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [BsonElement("description")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [BsonElement("poster_id")]
    [JsonPropertyName("poster_id")]
    public string PosterId { get; set; } = null!;

    [BsonElement("location")]
    [JsonPropertyName("location")]
    public string Location { get; set; } = null!;

    [BsonElement("category")]
    [JsonPropertyName("category")]
    public string Category { get; set; } = null!;

    [BsonElement("created_at")]
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Soa.ChatService.Models;

public class ChatMessageDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonPropertyName("_id")]
    public string Id { get; set; } = null!;

    [BsonElement("adId")]
    [JsonPropertyName("adId")]
    public string AdId { get; set; } = null!;

    [BsonElement("userId")]
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = null!;

    [BsonElement("userEmail")]
    [JsonPropertyName("userEmail")]
    public string UserEmail { get; set; } = null!;

    [BsonElement("message")]
    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;

    [BsonElement("timestamp")]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

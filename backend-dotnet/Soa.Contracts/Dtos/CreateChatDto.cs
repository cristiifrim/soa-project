using System.Text.Json.Serialization;

namespace Soa.Contracts.Dtos;

public class CreateChatDto
{
    [JsonPropertyName("adId")]
    public string AdId { get; set; } = null!;

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = null!;

    [JsonPropertyName("userEmail")]
    public string UserEmail { get; set; } = null!;

    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}

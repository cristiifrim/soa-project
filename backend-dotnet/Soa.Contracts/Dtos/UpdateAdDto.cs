using System.Text.Json.Serialization;

namespace Soa.Contracts.Dtos;

public class UpdateAdDto
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("poster_id")]
    public string? PosterId { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }
}

using System.Text.Json.Serialization;

namespace Soa.Contracts.Dtos;

public class CreateAdDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [JsonPropertyName("poster_id")]
    public string PosterId { get; set; } = null!;

    [JsonPropertyName("location")]
    public string Location { get; set; } = null!;

    [JsonPropertyName("category")]
    public string Category { get; set; } = null!;
}

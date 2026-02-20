using System.Text.Json.Serialization;

namespace Soa.Contracts.Dtos;

public class UpdateUserDto
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }
}

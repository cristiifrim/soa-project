using System.Text.Json.Serialization;

namespace Soa.Contracts.Dtos;

public class AuthLoginDto
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}

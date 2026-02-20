using System.Text.Json.Serialization;

namespace Soa.Contracts.Dtos;

public class CreateUserDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}

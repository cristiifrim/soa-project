namespace Soa.Contracts.Settings;

public class JwtSettings
{
    public string Secret { get; set; } = "your-secret-key";
    public int ExpiresInMinutes { get; set; } = 60;
}

namespace Soa.Contracts.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://mongodb:27017";
    public string DatabaseName { get; set; } = "ad-tracker-sandbox";
}

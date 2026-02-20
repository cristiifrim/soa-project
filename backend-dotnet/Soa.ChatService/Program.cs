using Soa.ChatService.Services;
using Soa.Contracts.Settings;

var builder = WebApplication.CreateBuilder(args);

var mongoSettings = builder.Configuration.GetSection("MongoDb").Get<MongoDbSettings>() ?? new MongoDbSettings();

builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<RabbitMqPublisher>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<RabbitMqPublisher>>();
    return RabbitMqPublisher.CreateAsync(config, logger).GetAwaiter().GetResult();
});

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

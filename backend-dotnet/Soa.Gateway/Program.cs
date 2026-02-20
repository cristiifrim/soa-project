using Soa.Gateway.Hubs;
using Soa.Gateway.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddHostedService<RabbitMqConsumerService>();
builder.Services.AddHostedService<KafkaConsumerService>();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:4201",
                "http://localhost:4202"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseRouting();  // ✅ important for endpoint-based CORS + SignalR

app.UseCors();     // ✅ must be after routing, before MapHub/MapReverseProxy

app.MapHub<ChatHub>("/hubs/chat").RequireCors();                  // ✅ ensure policy applies
app.MapHub<NotificationsHub>("/hubs/notifications").RequireCors(); // ✅ ensure policy applies

app.MapReverseProxy().RequireCors(); // ✅ if browser calls your proxied APIs too

app.Run();
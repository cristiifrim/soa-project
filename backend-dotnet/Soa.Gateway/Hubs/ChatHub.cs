using Microsoft.AspNetCore.SignalR;

namespace Soa.Gateway.Hubs;

public class ChatHub : Hub
{
    public async Task JoinChat(string adId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, adId);
    }

    public async Task SendMessage(string adId, string userId, string userEmail, string message)
    {
        var payload = new { adId, userId, userEmail, message, timestamp = DateTime.UtcNow };
        await Clients.Group(adId).SendAsync("NewMessage", payload);
    }
}

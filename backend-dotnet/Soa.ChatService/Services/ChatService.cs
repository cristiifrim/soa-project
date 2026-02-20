using MongoDB.Driver;
using Soa.ChatService.Models;
using Soa.Contracts.Dtos;
using Soa.Contracts.Settings;

namespace Soa.ChatService.Services;

public class ChatService
{
    private readonly IMongoCollection<ChatMessageDocument> _messages;

    public ChatService(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _messages = database.GetCollection<ChatMessageDocument>("chatmessages");
    }

    public async Task<ChatMessageDocument> CreateMessageAsync(CreateChatDto dto)
    {
        var message = new ChatMessageDocument
        {
            AdId = dto.AdId,
            UserId = dto.UserId,
            UserEmail = dto.UserEmail,
            Message = dto.Message,
            Timestamp = dto.Timestamp
        };

        await _messages.InsertOneAsync(message);
        return message;
    }

    public async Task<List<ChatMessageDocument>> GetMessagesByAdIdAsync(string adId)
    {
        return await _messages
            .Find(m => m.AdId == adId)
            .SortBy(m => m.Timestamp)
            .ToListAsync();
    }
}

using MongoDB.Driver;
using Soa.AdsService.Models;
using Soa.Contracts.Dtos;
using Soa.Contracts.Settings;

namespace Soa.AdsService.Services;

public class AdsService
{
    private readonly IMongoCollection<AdDocument> _ads;

    public AdsService(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _ads = database.GetCollection<AdDocument>("ads");
    }

    public async Task<AdDocument> CreateAsync(CreateAdDto dto)
    {
        var ad = new AdDocument
        {
            Title = dto.Title,
            Description = dto.Description,
            PosterId = dto.PosterId,
            Location = dto.Location,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _ads.InsertOneAsync(ad);
        return ad;
    }

    public async Task<List<AdDocument>> GetAllAsync()
    {
        return await _ads.Find(_ => true).ToListAsync();
    }

    public async Task<AdDocument?> GetByIdAsync(string id)
    {
        return await _ads.Find(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<AdDocument?> UpdateAsync(string id, UpdateAdDto dto)
    {
        var update = Builders<AdDocument>.Update;
        var updates = new List<UpdateDefinition<AdDocument>>();

        if (dto.Title is not null) updates.Add(update.Set(a => a.Title, dto.Title));
        if (dto.Description is not null) updates.Add(update.Set(a => a.Description, dto.Description));
        if (dto.PosterId is not null) updates.Add(update.Set(a => a.PosterId, dto.PosterId));
        if (dto.Location is not null) updates.Add(update.Set(a => a.Location, dto.Location));
        if (dto.Category is not null) updates.Add(update.Set(a => a.Category, dto.Category));
        updates.Add(update.Set(a => a.UpdatedAt, DateTime.UtcNow));

        if (updates.Count == 0) return await GetByIdAsync(id);

        return await _ads.FindOneAndUpdateAsync<AdDocument, AdDocument>(
            a => a.Id == id,
            update.Combine(updates),
            new FindOneAndUpdateOptions<AdDocument, AdDocument> { ReturnDocument = ReturnDocument.After });
    }

    public async Task<AdDocument?> DeleteAsync(string id)
    {
        return await _ads.FindOneAndDeleteAsync(a => a.Id == id);
    }
}

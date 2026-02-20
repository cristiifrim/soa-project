using System.ComponentModel.DataAnnotations;
using MongoDB.Driver;
using Soa.Contracts.Dtos;
using Soa.Contracts.Settings;
using Soa.UserService.Exceptions;
using Soa.UserService.Models;

namespace Soa.UserService.Services;

public class UserService
{
    private readonly IMongoCollection<UserDocument> _users;

    public UserService(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _users = database.GetCollection<UserDocument>("users");
    }

    public async Task<object> SignupAsync(CreateUserDto dto)
    {
        if (!new EmailAddressAttribute().IsValid(dto.Email))
            throw new UserServiceException("email", "E-mail isn't in a valid format");

        var existing = await _users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
        if (existing is not null)
            throw new UserServiceException("email", $"E-mail {dto.Email} already exists");

        var user = new UserDocument
        {
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password, 10)
        };

        await _users.InsertOneAsync(user);
        return new { message = "User created !", user_id = user.Id };
    }

    public async Task<object> LoginAsync(CreateUserDto dto)
    {
        if (!new EmailAddressAttribute().IsValid(dto.Email))
            throw new UserServiceException("email", "E-mail isn't in a valid format");

        var user = await _users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
        if (user is null)
            throw new UserServiceException("email", $"E-mail {dto.Email} doesn't exist");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            throw new UserServiceException("password", "Password is incorrect");

        return new { message = "User logged in !", user_id = user.Id };
    }

    public async Task<List<UserDocument>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

    public async Task<UserDocument?> GetByIdAsync(string id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<UserDocument?> UpdateAsync(string id, UpdateUserDto dto)
    {
        var update = Builders<UserDocument>.Update;
        var updates = new List<UpdateDefinition<UserDocument>>();

        if (dto.Email is not null) updates.Add(update.Set(u => u.Email, dto.Email));
        if (dto.Password is not null) updates.Add(update.Set(u => u.Password, BCrypt.Net.BCrypt.HashPassword(dto.Password, 10)));

        if (updates.Count == 0) return await GetByIdAsync(id);

        var combined = update.Combine(updates);
        return await _users.FindOneAndUpdateAsync<UserDocument, UserDocument>(
            u => u.Id == id,
            combined,
            new FindOneAndUpdateOptions<UserDocument, UserDocument> { ReturnDocument = ReturnDocument.After });
    }

    public async Task<UserDocument?> DeleteAsync(string id)
    {
        return await _users.FindOneAndDeleteAsync(u => u.Id == id);
    }

    public async Task<UserDocument?> FindByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }
}

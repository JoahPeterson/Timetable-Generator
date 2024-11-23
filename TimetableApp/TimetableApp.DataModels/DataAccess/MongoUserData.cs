using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableApp.DataModels.Models;

namespace TimetableApp.DataModels.DataAccess;
public class MongoUserData : IUserData
{
    private readonly IMongoCollection<User> _users;
    private readonly ILogger<MongoUserData> _logger;

    public MongoUserData(IDbConnection db, ILogger<MongoUserData> logger)
    {
        _users = db.UserCollection;
        _logger = logger;
    }

    public Task CreateAsync(User user)
    {
        try
        {
            return _users.InsertOneAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user");
            return null;
        }
        
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        try
        {
            var results = await _users.FindAsync(u => u.Id == id);
            return results.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user with Id of {UserId}", id);
            return null;
        }
        
    }

    public async Task<List<User>> GetUsersAsync()
    {
        try
        {
            var results = await _users.FindAsync(_ => true);
            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get users");
            return null;
        }
       
    }

    public async Task<User> GetUserFromAuthenticationAsync(string objectId)
    {
        try
        {
            var results = await _users.FindAsync(u => u.ObjectIdentifier == objectId);
            return results.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user from authentication with Id of {ObjectId}", objectId);
            return null;
        }
       
    }

    public Task UpdateAsync(User user)
    {
        try
        {
            var filter = Builders<User>.Filter.Eq("Id", user.Id);
            return _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user with Id of {UserId}", user.Id);
            return null;
        }
       
    }
}

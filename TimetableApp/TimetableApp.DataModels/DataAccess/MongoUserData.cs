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

    public MongoUserData(IDbConnection db)
    {
        _users = db.UserCollection;
    }

    public Task CreateAsync(User user)
    {
        return _users.InsertOneAsync(user);
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        var results = await _users.FindAsync(u => u.Id == id);
        return results.FirstOrDefault();
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var results = await _users.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<User> GetUserFromAuthenticationAsync(string objectId)
    {
        var results = await _users.FindAsync(u => u.ObjectIdentifier == objectId);
        return results.FirstOrDefault();
    }

    public Task UpdateAsync(User user)
    {
        var filter = Builders<User>.Filter.Eq("Id", user.Id);
        return _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
    }
}

using System.Linq.Expressions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using MongoDB.Driver;

namespace MongoAccess.Impl;
public class UserProfileSet : IUcSet<UserProfile>
{
    private readonly IMongoCollection<UserProfile> entities_;

    public UserProfileSet(IMongoCollection<UserProfile> entities)
    {
        entities_ = entities;
    }
    public Task AddAsync(UserProfile entity)
    {
        return entities_.InsertOneAsync(entity);
    }

    public async Task<UserProfile?> FirstOrDefaultAsync(Expression<Func<UserProfile, bool>> predicate)
    {
        return await (await entities_.FindAsync(predicate)).FirstOrDefaultAsync();
    }

    public Task RemoveAsync(UserProfile entity)
    {
        return entities_.FindOneAndDeleteAsync(u => u.UserId == entity.UserId);
    }

    public Task UpdateAsync(UserProfile entity)
    {
        return entities_.FindOneAndReplaceAsync(u => u.UserId == entity.UserId, entity);
    }
}

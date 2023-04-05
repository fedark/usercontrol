using System.Linq.Expressions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using MongoDB.Driver;

namespace MongoAccess.Impl;
public class UserSet : IUcSet<User>
{
    private readonly IMongoCollection<User> entities_;

    public UserSet(IMongoCollection<User> entities)
    {
        entities_ = entities;
    }
    public Task AddAsync(User entity)
    {
        return entities_.InsertOneAsync(entity);
    }

    public async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
    {
        return await (await entities_.FindAsync(predicate)).FirstOrDefaultAsync();
    }

    public Task RemoveAsync(User entity)
    {
        return entities_.FindOneAndDeleteAsync(u => u.Id == entity.Id);
    }

    public Task UpdateAsync(User entity)
    {
        return entities_.FindOneAndReplaceAsync(u => u.Id == entity.Id, entity);
    }
}

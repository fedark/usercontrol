using System.Linq.Expressions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using MongoDB.Driver;

namespace MongoAccess.Impl;
public class RoleSet : IUcSet<Role>
{
    private readonly IMongoCollection<Role> entities_;

    public RoleSet(IMongoCollection<Role> entities)
    {
        entities_ = entities;
    }
    public Task AddAsync(Role entity)
    {
        return entities_.InsertOneAsync(entity);
    }

    public async Task<Role?> FirstOrDefaultAsync(Expression<Func<Role, bool>> predicate)
    {
        return await (await entities_.FindAsync(predicate)).FirstOrDefaultAsync();
    }

    public Task RemoveAsync(Role entity)
    {
        return entities_.FindOneAndDeleteAsync(u => u.Id == entity.Id);
    }

    public Task UpdateAsync(Role entity)
    {
        return entities_.FindOneAndReplaceAsync(u => u.Id == entity.Id, entity);
    }
}

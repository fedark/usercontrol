using Data.Infrastructure.Abstractions;
using Data.Models;
using MongoDB.Driver;

namespace MongoAccess.Impl;
public class MongoUcContext : IUcContext
{
    private readonly IMongoDatabase database_;

    public IUcSet<User> Users => new UserSet(database_.GetCollection<User>("users"));
    public IUcSet<Role> Roles => new RoleSet(database_.GetCollection<Role>("roles"));
    public IUcSet<UserProfile> UserProfiles => new UserProfileSet(database_.GetCollection<UserProfile>("userprofiles"));

    public MongoUcContext(IMongoDatabase database)
    {
        database_ = database;
    }

    public void Dispose()
    {
    }

    public void SaveChanges()
    {
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

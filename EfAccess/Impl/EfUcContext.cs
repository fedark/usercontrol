using Data.Infrastructure.Abstractions;
using Data.Models;
using EfAccess.Infrastructure;

namespace EfAccess.Impl;
public class EfUcContext : IUcContext
{
    private readonly EfDbContext internalContext_;

    public IUcSet<User> Users => new UserSet(internalContext_);
    public IUcSet<Role> Roles => new RoleSet(internalContext_);
    public IUcSet<UserProfile> UserProfiles => new UserProfileSet(internalContext_);

    public EfUcContext(EfDbContext dbContext)
    {
        internalContext_ = dbContext;
    }

    public void SaveChanges()
    {
        internalContext_.SaveChanges();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return internalContext_.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        internalContext_.Dispose();
    }
}

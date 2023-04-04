using Data.Infrastructure.Abstractions;
using Data.Models;
using Ef.Infrastructure;

namespace Ef.Impl;
public class EfUcContext : IUcContext
{
    private readonly EfDbContext internalContext_;

    public IUcSet<User> Users => new UserDataSet(internalContext_);
    public IUcSet<Role> Roles => new RoleDataSet(internalContext_);
    public IUcSet<UserProfile> UserProfiles => new UserProfileDataSet(internalContext_);

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

using Data.Infrastructure.Abstractions;
using Data.Infrastructure.Services;
using Data.Models;
using Ef.Db;

namespace Ef.Infrastructure;
public class EfDataContext : IDataContext
{
    private readonly EfDbContext internalContext_;

    public IDataSet<User> Users => new UserDataSet(internalContext_);
    public IDataSet<Role> Roles => new RoleDataSet(internalContext_);
    public IDataSet<UserProfile> UserProfiles => new UserProfileDataSet(internalContext_);

    public EfDataContext(EfDbContext dbContext)
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

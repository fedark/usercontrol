using Data.Models;

namespace Data.Infrastructure.Abstractions;
public interface IDataContext : IDisposable
{
    IDataSet<User> Users { get; }
    IDataSet<Role> Roles { get; }
    IDataSet<UserProfile> UserProfiles { get; }

    void SaveChanges();
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

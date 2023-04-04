using Data.Models;

namespace Data.Infrastructure.Abstractions;
public interface IUcContext : IDisposable
{
    IUcSet<User> Users { get; }
    IUcSet<Role> Roles { get; }
    IUcSet<UserProfile> UserProfiles { get; }

    void SaveChanges();
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

using Data.Infrastructure.Abstractions;
using Data.Models;
using Microsoft.Data.SqlClient;

namespace DapperAccess.Impl;
public class DapperUcContext : IUcContext
{
    private readonly SqlConnection connection_;
    public IUcSet<User> Users => new UserSet(connection_);
    public IUcSet<Role> Roles => new RoleSet(connection_);
    public IUcSet<UserProfile> UserProfiles => new UserProfileSet(connection_);

    public DapperUcContext(SqlConnection connection)
    {
        connection_ = connection;
        connection_.Open();
    }

    public void Dispose()
    {
        connection_.Close();
        connection_.Dispose();
    }

    public void SaveChanges()
    {
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

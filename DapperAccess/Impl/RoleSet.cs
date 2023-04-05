using System.Linq.Expressions;
using Dapper.Contrib.Extensions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using Microsoft.Data.SqlClient;

namespace DapperAccess.Impl;
public class RoleSet : IUcSet<Role>
{
    private readonly SqlConnection connection_;

    public RoleSet(SqlConnection connection)
    {
        connection_ = connection;
    }
    public Task AddAsync(Role entity)
    {
        return connection_.InsertAsync(entity);
    }

    public async Task<Role?> FirstOrDefaultAsync(Expression<Func<Role, bool>> predicate)
    {
        return (await connection_.GetAllAsync<Role>()).FirstOrDefault(predicate.Compile());
    }

    public Task RemoveAsync(Role entity)
    {
        return connection_.DeleteAsync(entity);
    }

    public Task UpdateAsync(Role entity)
    {
        return connection_.UpdateAsync(entity);
    }
}

using System.Linq.Expressions;
using Dapper.Contrib.Extensions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using Microsoft.Data.SqlClient;

namespace DapperAccess.Impl;
public class UserSet : IUcSet<User>
{
    private readonly SqlConnection connection_;

    public UserSet(SqlConnection connection)
    {
        connection_ = connection;
    }
    public Task AddAsync(User entity)
    {
        return connection_.InsertAsync(entity);
    }

    public async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
    {
        return (await connection_.GetAllAsync<User>()).FirstOrDefault(predicate.Compile());
    }

    public Task RemoveAsync(User entity)
    {
        return connection_.DeleteAsync(entity);
    }

    public Task UpdateAsync(User entity)
    {
        return connection_.UpdateAsync(entity);
    }
}

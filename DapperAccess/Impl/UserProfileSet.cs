using System.Linq.Expressions;
using Dapper.Contrib.Extensions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using Microsoft.Data.SqlClient;

namespace DapperAccess.Impl;
public class UserProfileSet : IUcSet<UserProfile>
{
    private readonly SqlConnection connection_;

    public UserProfileSet(SqlConnection connection)
    {
        connection_ = connection;
    }
    public Task AddAsync(UserProfile entity)
    {
        return connection_.InsertAsync(entity);
    }

    public async Task<UserProfile?> FirstOrDefaultAsync(Expression<Func<UserProfile, bool>> predicate)
    {
        return (await connection_.GetAllAsync<UserProfile>()).FirstOrDefault(predicate.Compile());
    }

    public Task RemoveAsync(UserProfile entity)
    {
        return connection_.DeleteAsync(entity);
    }

    public Task UpdateAsync(UserProfile entity)
    {
        return connection_.UpdateAsync(entity);
    }
}

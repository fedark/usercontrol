using System.Linq.Expressions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using EfAccess.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EfAccess.Impl;
public class UserSet : IUcSet<User>
{
    private readonly EfDbContext context_;

    public UserSet(EfDbContext context)
    {
        context_ = context;
    }

    public async Task AddAsync(User entity)
    {
        await context_.Users.AddAsync(entity);
    }

    public Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
    {
        return context_.Users.FirstOrDefaultAsync(predicate);
    }

    public Task UpdateAsync(User entity)
    {
        context_.Update(entity);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(User entity)
    {
        context_.Users.Remove(entity);
        return Task.CompletedTask;
    }
}

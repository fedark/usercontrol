using System.Linq.Expressions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using EfAccess.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EfAccess.Impl;
public class RoleSet : IUcSet<Role>
{
    private readonly EfDbContext context_;

    public RoleSet(EfDbContext context)
    {
        context_ = context;
    }

    public async Task AddAsync(Role entity)
    {
        await context_.Roles.AddAsync(entity);
    }

    public Task<Role?> FirstOrDefaultAsync(Expression<Func<Role, bool>> predicate)
    {
        return context_.Roles.FirstOrDefaultAsync(predicate);
    }

    public Task UpdateAsync(Role entity)
    {
        context_.Update(entity);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Role entity)
    {
        context_.Roles.Remove(entity);
        return Task.CompletedTask;
    }
}

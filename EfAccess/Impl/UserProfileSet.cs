using System.Linq.Expressions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using EfAccess.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EfAccess.Impl;
public class UserProfileSet : IUcSet<UserProfile>
{
    private readonly EfDbContext context_;

    public UserProfileSet(EfDbContext context)
    {
        context_ = context;
    }

    public async Task AddAsync(UserProfile entity)
    {
        await context_.UserProfiles.AddAsync(entity);
    }

    public Task<UserProfile?> FirstOrDefaultAsync(Expression<Func<UserProfile, bool>> predicate)
    {
        return context_.UserProfiles.FirstOrDefaultAsync(predicate);
    }

    public Task UpdateAsync(UserProfile entity)
    {
        context_.Update(entity);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(UserProfile entity)
    {
        context_.UserProfiles.Remove(entity);
        return Task.CompletedTask;
    }
}

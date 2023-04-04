using System.Linq.Expressions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using Ef.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ef.Impl;
public class UserDataSet : IUcSet<User>
{
    private readonly EfDbContext context_;

    public UserDataSet(EfDbContext context)
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

    public void Update(User entity)
    {
        context_.Update(entity);
    }

    public void Remove(User entity)
    {
        context_.Users.Remove(entity);
    }
}

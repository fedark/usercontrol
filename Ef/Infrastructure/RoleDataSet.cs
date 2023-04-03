using System.Linq.Expressions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using Ef.Db;
using Microsoft.EntityFrameworkCore;

namespace Ef.Infrastructure;
public class RoleDataSet : IDataSet<Role>
{
    private readonly EfDbContext context_;

    public RoleDataSet(EfDbContext context)
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

    public void Update(Role entity)
    {
        context_.Update(entity);
    }

    public void Remove(Role entity)
    {
        context_.Roles.Remove(entity);
    }
}

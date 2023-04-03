﻿using System.Linq.Expressions;
using Data.Infrastructure.Abstractions;
using Data.Models;
using Ef.Db;
using Microsoft.EntityFrameworkCore;

namespace Ef.Infrastructure;
public class UserProfileDataSet : IDataSet<UserProfile>
{
    private readonly EfDbContext context_;

    public UserProfileDataSet(EfDbContext context)
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

    public void Update(UserProfile entity)
    {
        context_.Update(entity);
    }

    public void Remove(UserProfile entity)
    {
        context_.UserProfiles.Remove(entity);
    }
}

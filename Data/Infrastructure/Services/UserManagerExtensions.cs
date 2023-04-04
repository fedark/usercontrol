using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data.Infrastructure.Services;

public static class UserManagerExtensions
{
    public static async Task<bool> IsInAdminRoleAsync<TUser>(this UserManager<TUser> userManager, TUser user) where TUser : class
    {
        return await userManager.IsInRoleAsync(user, Role.Admin) || await userManager.IsInOwnerRoleAsync(user);
    }

    public static async Task<bool> IsInAdminRoleAsync<TUser>(this UserManager<TUser> userManager, string userId) where TUser : IdentityUser
    {
        var user = await userManager.Users.SingleAsync(u => u.Id == userId);
        return await userManager.IsInAdminRoleAsync(user);
    }

    public static Task<bool> IsInOwnerRoleAsync<TUser>(this UserManager<TUser> userManager, TUser user) where TUser : class
    {
        return userManager.IsInRoleAsync(user, Role.Owner);
    }

    public static async Task<bool> IsInOwnerRoleAsync<TUser>(this UserManager<TUser> userManager, string userId) where TUser : IdentityUser
    {
        var user = await userManager.Users.SingleAsync(u => u.Id == userId);
        return await userManager.IsInOwnerRoleAsync(user);
    }

    public static Task<IdentityResult> AddToAdminRoleAsync<TUser>(this UserManager<TUser> userManager, TUser user) where TUser : class
    {
        return userManager.AddToRoleAsync(user, Role.Admin);
    }

    public static Task<IdentityResult> RemoveFromAdminRoleAsync<TUser>(this UserManager<TUser> userManager, TUser user) where TUser : class
    {
        return userManager.RemoveFromRoleAsync(user, Role.Admin);
    }
}

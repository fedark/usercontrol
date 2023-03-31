using Data.Db;
using Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Data.Services;

public static class UserManagerExtensions
{
    public static async Task<bool> IsInAdminRoleAsync(this UserManager<User> userManager, User user)
    {
        return await userManager.IsInOwnerRoleAsync(user) &&
            await userManager.IsInRoleAsync(user, AppDbContext.AdminName);
    }

    public static Task<bool> IsInOwnerRoleAsync(this UserManager<User> userManager, User user)
    {
        return userManager.IsInRoleAsync(user, AppDbContext.OwnerName);
    }
}

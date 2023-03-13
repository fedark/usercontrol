using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserControl.Data;

namespace UserControl.Services;

public class AdminRoleManager
{
    private readonly RoleManager<IdentityRole> roleManager_;
    private readonly AppDbContext context_;

    public AdminRoleManager(RoleManager<IdentityRole> roleManager, AppDbContext context)
    {
        roleManager_ = roleManager;
        context_ = context;
    }

    public async Task<bool> IsAdminAsync(string userId)
    {
        var adminRole = await roleManager_.FindByNameAsync(AppDbContext.AdminName);

        return await IsPrimeAdminAsync(userId) || 
            await context_.UserRoles.ContainsAsync(new() { UserId = userId, RoleId = adminRole.Id });
    }

    public async Task<bool> IsPrimeAdminAsync(string userId)
    {
        var primeAdminRole = await roleManager_.FindByNameAsync(AppDbContext.PrimeAdminName);

        return await context_.UserRoles.ContainsAsync(new() { UserId = userId, RoleId = primeAdminRole.Id });
    }
}

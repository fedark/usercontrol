using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserControl.Data;

namespace UserControl.Access;

public class NotPrimeAdminRequirement : IAuthorizationRequirement
{
}

public class NotPrimeAdminHandler : AuthorizationHandler<NotPrimeAdminRequirement>
{
    private readonly RoleManager<IdentityRole> roleManager_;
    private readonly AppDbContext context_;

    public NotPrimeAdminHandler(RoleManager<IdentityRole> roleManager, AppDbContext context)
    {
        roleManager_ = roleManager;
        context_ = context;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotPrimeAdminRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var id = httpContext.Request.RouteValues["id"];
            var primeAdminRole = await roleManager_.FindByNameAsync(AppDbContext.PrimeAdminName);

            if (id is string userId && 
                (await context_.UserRoles.ContainsAsync(new() { UserId = userId, RoleId = primeAdminRole.Id })))
            {
                context.Fail();              
            }
            else
            {
                context.Succeed(requirement);
            }
        }
    }
}

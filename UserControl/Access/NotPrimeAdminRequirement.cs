using Microsoft.AspNetCore.Authorization;
using UserControl.Services;

namespace UserControl.Access;

public class NotPrimeAdminRequirement : IAuthorizationRequirement
{
}

public class NotPrimeAdminHandler : AuthorizationHandler<NotPrimeAdminRequirement>
{
    private readonly AdminRoleManager adminRoleManager_;

    public NotPrimeAdminHandler(AdminRoleManager adminRoleManager)
    {
        adminRoleManager_ = adminRoleManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotPrimeAdminRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var id = httpContext.Request.RouteValues["id"];

            if (id is string userId && await adminRoleManager_.IsPrimeAdminAsync(userId))
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

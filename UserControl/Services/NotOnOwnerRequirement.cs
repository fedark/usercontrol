using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace UserControl.Access;

public class NotOnOwnerRequirement : IAuthorizationRequirement
{
}

public class NotOnOwnerHandler : AuthorizationHandler<NotOnOwnerRequirement>
{
    private readonly UserManager<User> userManager_;

    public NotOnOwnerHandler(UserManager<User> userManager)
    {
        userManager_ = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotOnOwnerRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var id = httpContext.Request.RouteValues["id"];

            if (id is string userId && await userManager_.IsInOwnerRoleAsync(userId))
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

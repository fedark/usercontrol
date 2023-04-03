using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace UserControl.Access;

public class NotOwnerRequirement : IAuthorizationRequirement
{
}

public class NotOwnerHandler : AuthorizationHandler<NotOwnerRequirement>
{
    private readonly UserManager<User> userManager_;

    public NotOwnerHandler(UserManager<User> userManager)
    {
        userManager_ = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotOwnerRequirement requirement)
    {
        var user = await userManager_.GetUserAsync(context.User);
        if (user is not null && await userManager_.IsInOwnerRoleAsync(user))
        {
            context.Fail();
        }
        else
        {
            context.Succeed(requirement);
        }
    }
}

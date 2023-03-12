using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using UserControl.Data;

namespace UserControl.Access;

public class NotPrimeAdminRequirement : IAuthorizationRequirement
{
}

public class NotPrimeAdminHandler : AuthorizationHandler<NotPrimeAdminRequirement>
{
    private readonly UserManager<IdentityUser> userManager_;

    public NotPrimeAdminHandler(UserManager<IdentityUser> userManager)
    {
        userManager_ = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotPrimeAdminRequirement requirement)
    {
        var user = await userManager_.GetUserAsync(context.User);
        if (user.UserName != AppDbContext.AdminName)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}

using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace UserControl.Access;

public class NotPrimeAdminUserRequirement : IAuthorizationRequirement
{
}

public class NotPrimeAdminUserHandler : AuthorizationHandler<NotPrimeAdminUserRequirement>
{
    private readonly AdminRoleManager adminRoleManager_;
    private readonly UserManager<IdentityUser> userManager_;

    public NotPrimeAdminUserHandler(AdminRoleManager adminRoleManager, UserManager<IdentityUser> userManager)
    {
        adminRoleManager_ = adminRoleManager;
        userManager_ = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotPrimeAdminUserRequirement requirement)
    {
        var user = await userManager_.GetUserAsync(context.User);
        if (user is not null && await adminRoleManager_.IsPrimeAdminAsync(user.Id))
        {
            context.Fail();
        }
        else
        {
            context.Succeed(requirement);
        }
    }
}

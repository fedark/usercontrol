using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace UserControl.Access
{
    public class NotSelfUserRequirement : IAuthorizationRequirement
    {
    }

    public class NotSelfUserHandler : AuthorizationHandler<NotSelfUserRequirement>
    {
        private readonly UserManager<IdentityUser> userManager_;

        public NotSelfUserHandler(UserManager<IdentityUser> userManager)
        {
            userManager_ = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotSelfUserRequirement requirement)
        {
            if (context.Resource is HttpContext httpContext)
            {
                var id = httpContext.Request.RouteValues["id"];

                var user = await userManager_.GetUserAsync(context.User);
                if (id is string userId && user.Id != userId)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
        }
    }
}

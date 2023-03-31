using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace UserControl.Access
{
    public class NotOnSelfRequirement : IAuthorizationRequirement
    {
    }

    public class NotOnSelfHandler : AuthorizationHandler<NotOnSelfRequirement>
    {
        private readonly UserManager<User> userManager_;

        public NotOnSelfHandler(UserManager<User> userManager)
        {
            userManager_ = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotOnSelfRequirement requirement)
        {
            if (context.Resource is HttpContext httpContext)
            {
                var id = httpContext.Request.RouteValues["id"];

                var user = await userManager_.GetUserAsync(context.User);
                if (id is string userId && user.Id != userId)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UserControl.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> signInManager_;
        private readonly ILogger<LogoutModel> logger_;

        public LogoutModel(SignInManager<User> signInManager, ILogger<LogoutModel> logger)
        {
            signInManager_ = signInManager;
            logger_ = logger;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = default)
        {
            await signInManager_.SignOutAsync();
            logger_.LogInformation("User logged out.");

            if (returnUrl is not null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserControl.Services;

namespace UserControl.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Policy = Policy.NotOwner)]
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<User> userManager_;

        public PersonalDataModel(UserManager<User> userManager)
        {
            userManager_ = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await userManager_.GetUserAsync(User);
            if (user is null)
            {
                return NotFound($"Unable to load user with ID '{userManager_.GetUserId(User)}'.");
            }

            return Page();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using UserControl.Services;

namespace UserControl.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Policy = Policy.NotOwner)]
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<User> userManager_;
        private readonly SignInManager<User> signInManager_;
        private readonly ILogger<DeletePersonalDataModel> logger_;
        private readonly IStringLocalizer<DeletePersonalDataModel> localizer_;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public bool RequirePassword { get; set; }

        public DeletePersonalDataModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            IStringLocalizer<DeletePersonalDataModel> localizer)
        {
            userManager_ = userManager;
            signInManager_ = signInManager;
            logger_ = logger;
            localizer_ = localizer;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await userManager_.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(localizer_["UserNotFound", userManager_.GetUserId(User)]);
            }

            RequirePassword = await userManager_.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager_.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(localizer_["UserNotFound", userManager_.GetUserId(User)]);
            }

            RequirePassword = await userManager_.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await userManager_.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, localizer_["IncorrectPassword"]);
                    return Page();
                }
            }

            var result = await userManager_.DeleteAsync(user);
            var userId = await userManager_.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(localizer_["DeleteError"]);
            }

            await signInManager_.SignOutAsync();

            logger_.LogInformation($"User with ID '{userId}' deleted themselves.");

            return Redirect("~/");
        }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = default!;
        }
    }
}

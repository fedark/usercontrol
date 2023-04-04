// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using Data.Models;
using Data.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data.Infrastructure.Abstractions;

namespace UserControl.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> signInManager_;
        private readonly UserManager<User> userManager_;
        private readonly ILogger<RegisterModel> logger_;
        private readonly IUcContext context_;
        private readonly UserProfileProvider userProfileProvider_;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string ReturnUrl { get; set; } = default!;

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = default!;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IUcContext context,
            UserProfileProvider userProfileProvider)
        {
            userManager_ = userManager;
            signInManager_ = signInManager;
            logger_ = logger;
            context_ = context;
            userProfileProvider_ = userProfileProvider;
        }

        public async Task OnGetAsync(string? returnUrl = default)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;
            ExternalLogins = (await signInManager_.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = default)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await signInManager_.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new User(Input.UserName);
                var result = await userManager_.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await CreateUserProfile(user);

                    logger_.LogInformation("User created a new account with password.");

                    await signInManager_.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private async Task CreateUserProfile(User user)
        {
            var userProfile = await userProfileProvider_.GetDefaultProfileAsync(user.Id);
            await context_.UserProfiles.AddAsync(userProfile);
            await context_.SaveChangesAsync();
        }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "UserName")]
            public string UserName { get; set; } = default!;

            [Required]
            [StringLength(100, ErrorMessage = "PasswordError", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = default!;

            [DataType(DataType.Password)]
            [Display(Name = "ConfirmPassword")]
            [Compare("Password", ErrorMessage = "PasswordConfirmationError")]
            public string ConfirmPassword { get; set; } = default!;
        }
    }
}

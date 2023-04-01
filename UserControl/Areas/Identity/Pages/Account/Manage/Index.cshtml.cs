// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using Data.Db;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace UserControl.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> userManager_;
        private readonly SignInManager<User> signInManager_;
        private readonly AppDbContext context_;
        private readonly UserProfileProvider userProfileProvider_;

        public string UserName { get; set; } = default!;

        [TempData]
        public string StatusMessage { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            AppDbContext context,
            UserProfileProvider userProfileProvider)
        {
            userManager_ = userManager;
            signInManager_ = signInManager;
            context_ = context;
            userProfileProvider_ = userProfileProvider;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager_.GetUserAsync(User);
            if (user is null)
            {
                return NotFound($"Unable to load user with ID '{userManager_.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager_.GetUserAsync(User);
            if (user is null)
            {
                return NotFound($"Unable to load user with ID '{userManager_.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Input.UserPicture?.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await Input.UserPicture.CopyToAsync(memoryStream);

                if (memoryStream.Length < 4 * 1024 * 1024)
                {
                    context_.Entry(user).Reference(u => u.UserProfile).Load();
                    user.UserProfile.Picture = memoryStream.ToArray();
                    user.UserProfile.PictureType = Input.UserPicture.ContentType;

                    await userManager_.UpdateAsync(user);
                }
                else
                {
                    StatusMessage = "The user picture must be less than 4MB.";
                    return RedirectToPage();
                }
            }

            await signInManager_.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeletePictureAsync()
        {
            var user = await userManager_.GetUserAsync(User);
            if (user is null)
            {
                return NotFound($"Unable to load user with ID '{userManager_.GetUserId(User)}'.");
            }

            var defaultProfile = await userProfileProvider_.GetDefaultProfileAsync(user.Id);
            var userProfile = await context_.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (userProfile is null)
            {
                userProfile = defaultProfile;
            }
            else
            {
                userProfile.Picture = defaultProfile.Picture;
            }

            context_.UserProfiles.Update(userProfile);
            await context_.SaveChangesAsync();

            await signInManager_.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";

            return RedirectToPage();
        }

        private async Task LoadAsync(User user)
        {
            UserName = await userManager_.GetUserNameAsync(user);
        }

        public class InputModel
        {
            [Display(Name = "User Picture")]
            [DataType(DataType.Upload)]
            public IFormFile UserPicture { get; set; } = default;
        }
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using Data.Infrastructure.Abstractions;
using Data.Infrastructure.Services;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace UserControl.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> userManager_;
        private readonly SignInManager<User> signInManager_;
        private readonly IDataContext context_;
        private readonly UserProfileProvider userProfileProvider_;
        private readonly IStringLocalizer<IndexModel> localizer_;

        [Display(Name = "UserName")]
        public string UserName { get; set; } = default!;

        [TempData]
        public string StatusMessage { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IDataContext context,
            UserProfileProvider userProfileProvider,
            IStringLocalizer<IndexModel> localizer)
        {
            userManager_ = userManager;
            signInManager_ = signInManager;
            context_ = context;
            userProfileProvider_ = userProfileProvider;
            localizer_ = localizer;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager_.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(localizer_["UserNotFound", userManager_.GetUserId(User)]);
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager_.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(localizer_["UserNotFound", userManager_.GetUserId(User)]);
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
                    var userWithNavigation = await userManager_.Users.Where(u => u.Id == user.Id).Include(u => u.UserProfile).SingleAsync();
                    userWithNavigation.UserProfile.Picture = memoryStream.ToArray();
                    userWithNavigation.UserProfile.PictureType = Input.UserPicture.ContentType;

                    await userManager_.UpdateAsync(userWithNavigation);
                }
                else
                {
                    StatusMessage = localizer_["PictureSizeMessage"];
                    return RedirectToPage();
                }
            }

            await signInManager_.RefreshSignInAsync(user);
            StatusMessage = localizer_["ProfileUpdated"];

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeletePictureAsync()
        {
            var user = await userManager_.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(localizer_["UserNotFound", userManager_.GetUserId(User)]);
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
            StatusMessage = localizer_["ProfileUpdated"];

            return RedirectToPage();
        }

        private async Task LoadAsync(User user)
        {
            UserName = await userManager_.GetUserNameAsync(user);
        }

        public class InputModel
        {
            [Display(Name = "UserPicture")]
            [DataType(DataType.Upload)]
            public IFormFile UserPicture { get; set; } = default!;
        }
    }
}

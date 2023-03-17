// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Data.Db;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UserControl.Services;

namespace UserControl.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserProfileProvider userProfileProvider_;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            AppDbContext context,
            UserProfileProvider userProfileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            userProfileProvider_ = userProfileProvider;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "User Picture")]
            [DataType(DataType.Upload)]
            public IFormFile UserPicture { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            Username = await _userManager.GetUserNameAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
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
                    var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id) ??
                        new UserProfile { UserId = user.Id, PictureType = Input.UserPicture.ContentType };
                    userProfile.Picture = memoryStream.ToArray();

                    _context.UserProfiles.Update(userProfile);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    StatusMessage = "The user picture must be less than 4MB.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeletePictureAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var defaultProfile = await userProfileProvider_.GetDefaultProfileAsync(user);
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (userProfile is null)
            {
                userProfile = defaultProfile;
            }
            else
            {
                userProfile.Picture = defaultProfile.Picture;
            }

            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}

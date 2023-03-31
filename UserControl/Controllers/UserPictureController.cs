using Data.Db;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace UserControl.Controllers
{
    public class UserPictureController : Controller
    {
        private readonly AppDbContext context_;

        public UserPictureController(AppDbContext context)
        {
            context_ = context;
        }

        public async Task<IActionResult> LoadUserPicture(string? id)
        {
            var user = await LoadUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            context_.Entry(user).Reference(u => u.UserProfile).Load();
            var userProfile = user.UserProfile;

            return File(userProfile.Picture, userProfile.PictureType);
        }

        private async Task<User?> LoadUserAsync(string? id)
        {
            if (id is null || context_.Users is null)
            {
                return null;
            }

            return await context_.Users.FindAsync(id);
        }
    }
}

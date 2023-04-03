using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace UserControl.Controllers
{
    public class UserPictureController : Controller
    {
        private readonly UserManager<User> userManager_;

        public UserPictureController(UserManager<User> userManager)
        {
            userManager_ = userManager;
        }

        public async Task<IActionResult> LoadUserPicture(string? id)
        {
            var user = await userManager_.Users.Where(u => u.Id == id).Include(u => u.UserProfile).FirstOrDefaultAsync();
            if (user is null)
            {
                return NotFound();
            }

            var userProfile = user.UserProfile;

            return File(userProfile.Picture, userProfile.PictureType);
        }
    }
}

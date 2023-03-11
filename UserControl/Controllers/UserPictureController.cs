using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserControl.Data;

namespace UserControl.Controllers
{
    public class UserPictureController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment_;

        public UserPictureController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            webHostEnvironment_ = webHostEnvironment;
        }

        public async Task<IActionResult> LoadUserPicture(string? id)
        {
            var user = await LoadUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (userProfile is not null)
            {
                return File(userProfile.Picture, userProfile.PictureType);
            }

            var webRoot = webHostEnvironment_.WebRootPath;
            var defaultPicturePath = Path.Combine(webRoot, "images", "no_user_picture.png");
            return File(new FileStream(defaultPicturePath, FileMode.Open), "image/png");
        }

        private async Task<IdentityUser?> LoadUserAsync(string? id)
        {
            if (id is null || _context.Users is null)
            {
                return null;
            }

            return await _context.Users.FindAsync(id);
        }
    }
}

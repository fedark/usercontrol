using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserControl.Data;
using UserControl.Models;

namespace UserControl.Controllers
{
    [Authorize]
    public partial class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> roleManager_;

		public UserController(AppDbContext context, RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			roleManager_ = roleManager;
		}

		public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(await LoadUsersAsync(users));
        }

        public async Task<IActionResult> Details(string? id)
        {
            var user = await LoadUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize(Roles = AppDbContext.AdminName, Policy = "NotSelf")]
        public async Task<IActionResult> Edit(string? id)
        {
            var user = await LoadUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppDbContext.AdminName, Policy = "NotSelf")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,IsAdmin")] DisplayUserModel user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await TryChangeRoleAsync(id, user.IsAdmin))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }

            return View(user);
        }

        [Authorize(Roles = AppDbContext.AdminName, Policy = "NotSelf")]
        public async Task<IActionResult> Delete(string? id)
        {
            var user = await LoadUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppDbContext.AdminName, Policy = "NotSelf")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users is null)
            {
                return Problem($"Entity set {nameof(_context.Users)} is null.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user is not null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TryChangeRoleAsync(string? id, bool makeAdmin)
        {
            var user = await LoadUserAsync(id);
            if (user is null)
            {
                return false;
            }

            var adminRole = await roleManager_.FindByNameAsync(AppDbContext.AdminName);
            if (!user.IsAdmin && makeAdmin)
            {
                await _context.UserRoles.AddAsync(new() { UserId = user.Id, RoleId = adminRole.Id });
            }
            else if (user.IsAdmin && !makeAdmin)
            {
                var userRole = await _context.UserRoles.FindAsync(user.Id, adminRole.Id);
                if (userRole is not null)
                {
                    _context.UserRoles.Remove(userRole);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<DisplayUserModel?> LoadUserAsync(string? id)
        {
            if (id is null || _context.Users is null)
            {
                return null;
            }

            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return null;
            }

            return await LoadUserAsync(user);
        }

        private async Task<DisplayUserModel> LoadUserAsync(IdentityUser user)
        {
			var adminRole = await roleManager_.FindByNameAsync(AppDbContext.AdminName);
			var isAdmin = await _context.UserRoles.ContainsAsync(new() { UserId = user.Id, RoleId = adminRole.Id });
            var displayUser = new DisplayUserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                IsAdmin = isAdmin
            };

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (userProfile is not null)
            {
                displayUser.Picture = userProfile.Picture;
                displayUser.PictureType = userProfile.PictureType;
            }

            return displayUser;
        }

        private async Task<IEnumerable<DisplayUserModel>> LoadUsersAsync(IEnumerable<IdentityUser> users)
        {
            var result = new List<DisplayUserModel>();

            foreach (var user in users)
            {
                result.Add(await LoadUserAsync(user));
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UserControl.Data;
using UserControl.Models;

namespace UserControl.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment hostEnvironment_;

        public UserController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            hostEnvironment_ = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(await ToDisplayUsersAsync(users));
        }

        public async Task<IActionResult> LoadUserPicture(string? id)
        {
            var user = await GetUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (userProfile is null)
            {
                return NotFound();
            }

            return File(userProfile.Picture, "image/jpg");
        }

        public async Task<IActionResult> Details(string? id)
        {
            var user = await GetUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize(Roles = AppDbContext.AdminName)]
        public async Task<IActionResult> Edit(string? id)
        {
            var user = await GetUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppDbContext.AdminName)]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,IsAdmin")] DisplayUserModel displayUser)
        {
            if (id != displayUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await TryChangeRoleAsync(id, displayUser.IsAdmin))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }

            return View(displayUser);
        }

        [Authorize(Roles = AppDbContext.AdminName)]
        public async Task<IActionResult> Delete(string? id)
        {
            var user = await GetUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppDbContext.AdminName)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
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
            var user = await GetUserAsync(id);
            if (user is null)
            {
                return false;
            }

            if (!user.IsAdmin && makeAdmin)
            {
                await _context.UserRoles.AddAsync(new() { UserId = user.Id, RoleId = _context.AdminRole.Id });
            }
            else if (user.IsAdmin && !makeAdmin)
            {
                var userRole = await _context.UserRoles.FindAsync(user.Id, _context.AdminRole.Id);
                if (userRole is not null)
                {
                    _context.UserRoles.Remove(userRole);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<DisplayUserModel?> GetUserAsync(string? id)
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

            return await ToDisplayUserAsync(user);
        }

        private async Task<DisplayUserModel> ToDisplayUserAsync(IdentityUser user)
        {
            var isAdmin = await _context.UserRoles.ContainsAsync(new() { UserId = user.Id, RoleId = _context.AdminRole.Id });
            return new() { Id = user.Id, UserName = user.UserName, Email = user.Email, IsAdmin = isAdmin };
        }

        private async Task<List<DisplayUserModel>> ToDisplayUsersAsync(IEnumerable<IdentityUser> users)
        {
            var result = new List<DisplayUserModel>();

            foreach (var user in users)
            {
                result.Add(await ToDisplayUserAsync(user));
            }

            return result;
        }
    }
}

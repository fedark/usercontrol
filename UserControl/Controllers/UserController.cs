using AutoMapper;
using Data.Db;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserControl.ViewModels;

namespace UserControl.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly AppDbContext context_;
        private readonly RoleManager<Role> roleManager_;
        private readonly UserManager<User> userManager_;
        private readonly IMapper mapper_;

        public UserController(AppDbContext context, 
            RoleManager<Role> roleManager, 
            UserManager<User> userManager,
            IMapper mapper)
        {
            context_ = context;
            roleManager_ = roleManager;
            userManager_ = userManager;
            mapper_ = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var users = await context_.Users.ToListAsync();
            var viewUsers = mapper_.Map<IEnumerable<User>>(users);

            return View(new UserListViewModel(viewUsers));
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

        [Authorize(Roles = $"{Role.Admin},{Role.Owner}")]
        [Authorize(Policy = "NotSelf")]
        [Authorize(Policy = "NotPrimeAdmin")]
        public async Task<IActionResult> Edit(string? id)
        {
            var user = await LoadUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(mapper_.Map<UserViewModel>(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Role.Admin},{Role.Owner}")]
        [Authorize(Policy = "NotSelf")]
        [Authorize(Policy = "NotPrimeAdmin")]
        public async Task<IActionResult> Edit(
            [Bind(nameof(UserViewModel.Id), nameof(UserViewModel.UserName), nameof(UserViewModel.IsAdmin))] 
            UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (await TryChangeRoleAsync(user))
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

        [Authorize(Roles = $"{Role.Admin},{Role.Owner}")]
        [Authorize(Policy = "NotSelf")]
        [Authorize(Policy = "NotPrimeAdmin")]
        public async Task<IActionResult> Delete(string? id)
        {
            var user = await LoadUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(mapper_.Map<UserViewModel>(user));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Role.Admin},{Role.Owner}")]
        [Authorize(Policy = "NotSelf")]
        [Authorize(Policy = "NotPrimeAdmin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await LoadUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            context_.Users.Remove(user);
            await context_.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TryChangeRoleAsync(UserViewModel viewUser)
        {
            var user = await LoadUserAsync(viewUser.Id);
            if (user is null)
            {
                return false;
            }

            if (await userManager_.IsInAdminRoleAsync(user))
            {
                await userManager_.AddToAdminRoleAsync(user);
            }
            else
            {
                await userManager_.RemoveFromAdminRoleAsync(user);
            }

            await context_.SaveChangesAsync();
            return true;
        }

        private async Task<User?> LoadUserAsync(string? id)
        {
            if (id is null || context_.Users is null)
            {
                return null;
            }

            var user = await context_.Users.FindAsync(id);
            if (user is null)
            {
                return null;
            }

            return user;
        }
    }
}

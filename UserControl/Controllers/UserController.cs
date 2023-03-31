using AutoMapper;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserControl.Services;
using UserControl.ViewModels;

namespace UserControl.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager_;
        private readonly IMapper mapper_;

        public UserController(UserManager<User> userManager,
            IMapper mapper)
        {
            userManager_ = userManager;
            mapper_ = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var users = await userManager_.Users.ToListAsync();
            var viewUsers = mapper_.Map<IEnumerable<UserViewModel>>(users);

            return View(new UserListViewModel(viewUsers));
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

        [Authorize(Roles = $"{Role.Admin},{Role.Owner}")]
        [Authorize(Policy = Policy.NotOnSelf)]
        [Authorize(Policy = Policy.NotOnOwner)]
        public async Task<IActionResult> Edit(string? id)
        {
            var user = await GetUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(mapper_.Map<UserViewModel>(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Role.Admin},{Role.Owner}")]
        [Authorize(Policy = Policy.NotOnSelf)]
        [Authorize(Policy = Policy.NotOnOwner)]
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
        [Authorize(Policy = Policy.NotOnSelf)]
        [Authorize(Policy = Policy.NotOnOwner)]
        public async Task<IActionResult> Delete(string? id)
        {
            var user = await GetUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(mapper_.Map<UserViewModel>(user));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Role.Admin},{Role.Owner}")]
        [Authorize(Policy = Policy.NotOnSelf)]
        [Authorize(Policy = Policy.NotOnOwner)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await GetUserAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            await userManager_.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TryChangeRoleAsync(UserViewModel viewUser)
        {
            var user = await GetUserAsync(viewUser.Id);
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

            return true;
        }

        private async Task<User?> GetUserAsync(string? id)
        {
            if (id is null)
            {
                return null;
            }

            return await userManager_.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserControl.Data;
using UserControl.Models;

namespace UserControl.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

        public async Task<IActionResult> LoadUserPictureAsync(string? id)
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

        private async Task<IdentityUser?> GetUserAsync(string? id)
        {
            if (id is null || _context.Users is null)
            {
                return null;
            }

            return await _context.Users.FindAsync(id);
        }
    }
}
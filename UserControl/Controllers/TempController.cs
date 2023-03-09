using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserControl.Data;

namespace UserControl.Controllers
{
    public class TempController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager_;
        private readonly AppDbContext _context;

        public TempController(RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            roleManager_ = roleManager;
            _context = context;
        }

        // GET: TempController
        public async Task<IActionResult> Index()
        {
            await roleManager_.CreateAsync(new IdentityRole { Name = "admin" });
            return View();
        }
    }
}

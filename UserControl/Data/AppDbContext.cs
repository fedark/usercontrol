using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserControl.Models;

namespace UserControl.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public const string AdminName = "admin";
        public IdentityRole AdminRole { get; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
            AdminRole = Roles.First(r => r.Name == AdminName);
        }
    }
}

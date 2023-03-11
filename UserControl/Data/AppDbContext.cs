using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserControl.Models;

namespace UserControl.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public const string AdminName = "admin";

        public DbSet<UserProfile> UserProfiles { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

            var adminRole = new IdentityRole(AdminName) { NormalizedName = AdminName.ToUpper() };
			var adminUser = new IdentityUser(AdminName) { NormalizedUserName = AdminName.ToUpper() };

			var passwordHasher = new PasswordHasher<IdentityUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Ad!min0");

			builder.Entity<IdentityRole>().HasData(adminRole);
            builder.Entity<IdentityUser>().HasData(adminUser);
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { RoleId = adminRole.Id, UserId = adminUser.Id });
		}
	}
}

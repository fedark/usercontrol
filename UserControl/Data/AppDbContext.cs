using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserControl.Models;
using UserControl.Services;

namespace UserControl.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public const string AdminName = "admin";

        public DbSet<UserProfile> UserProfiles { get; set; }

        private readonly DefaultUserProfileProvider userProfileProvider_;

        public AppDbContext(DbContextOptions options, DefaultUserProfileProvider userProfileProvider) : base(options)
        {
            userProfileProvider_ = userProfileProvider;
        }

        protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

            var adminRole = new IdentityRole(AdminName) { NormalizedName = AdminName.ToUpper() };

			var adminUser = new IdentityUser(AdminName) { NormalizedUserName = AdminName.ToUpper() };
            var passwordHasher = new PasswordHasher<IdentityUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Ad!min0");

            var adminUserRole = new IdentityUserRole<string> { RoleId = adminRole.Id, UserId = adminUser.Id };
            var adminProfile = userProfileProvider_.GetDefaultProfile(adminUser);

            builder.Entity<IdentityRole>().HasData(adminRole);
            builder.Entity<IdentityUser>().HasData(adminUser);
            builder.Entity<IdentityUserRole<string>>().HasData(adminUserRole);
            builder.Entity<UserProfile>().HasData(adminProfile);
		}
	}
}

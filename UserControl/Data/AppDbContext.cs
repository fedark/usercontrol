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
        public const string PrimeAdminName = "prime_admin";

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
            var primeAdminRole = new IdentityRole(PrimeAdminName) { NormalizedName = PrimeAdminName.ToUpper() };

            var primeAdminUser = new IdentityUser(AdminName) { NormalizedUserName = AdminName.ToUpper() };
            var passwordHasher = new PasswordHasher<IdentityUser>();
            primeAdminUser.PasswordHash = passwordHasher.HashPassword(primeAdminUser, "Ad!min0");

            var primeAdminUserRole = new IdentityUserRole<string> { RoleId = primeAdminRole.Id, UserId = primeAdminUser.Id };
            var primeAdminProfile = userProfileProvider_.GetDefaultProfile(primeAdminUser);

            builder.Entity<IdentityRole>().HasData(adminRole);
            builder.Entity<IdentityRole>().HasData(primeAdminRole);
            builder.Entity<IdentityUser>().HasData(primeAdminUser);
            builder.Entity<IdentityUserRole<string>>().HasData(primeAdminUserRole);
            builder.Entity<UserProfile>().HasData(primeAdminProfile);
        }
    }
}

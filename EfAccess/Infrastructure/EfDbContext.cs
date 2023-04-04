using Data.Infrastructure.Services;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EfAccess.Infrastructure;

public class EfDbContext : IdentityDbContext<User, Role, string>
{
    public DbSet<UserProfile> UserProfiles { get; set; } = default!;

    private readonly IdentitySeedOptions seedOptions_;
    private readonly UserProfileProvider userProfileProvider_;

    public EfDbContext(DbContextOptions options, IOptions<IdentitySeedOptions> seedOptions, UserProfileProvider userProfileProvider) : base(options)
    {
        seedOptions_ = seedOptions.Value;
        userProfileProvider_ = userProfileProvider;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var adminRole = new Role(seedOptions_.AdminName);
        var ownerRole = new Role(seedOptions_.OwnerName);

        var ownerUser = new User(seedOptions_.OwnerName);
        var passwordHasher = new PasswordHasher<User>();
        ownerUser.PasswordHash = passwordHasher.HashPassword(ownerUser, seedOptions_.OwnerPassword);

        var ownerUserRole = new IdentityUserRole<string> { RoleId = ownerRole.Id, UserId = ownerUser.Id };
        var ownerProfile = userProfileProvider_.GetDefaultProfile(ownerUser.Id);

        builder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(p => p.UserId);

            entity.HasOne<User>()
                .WithOne(u => u.UserProfile)
                .HasForeignKey<UserProfile>(p => p.UserId);
        });

        builder.Entity<Role>().HasData(adminRole, ownerRole);
        builder.Entity<User>().HasData(ownerUser);
        builder.Entity<IdentityUserRole<string>>().HasData(ownerUserRole);
        builder.Entity<UserProfile>().HasData(ownerProfile);
    }
}

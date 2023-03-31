using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Data.Db;

public class AppDbContext : IdentityDbContext<User, Role, string>
{
    public const string AdminName = "admin";
    public const string OwnerName = "owner";

    public DbSet<UserProfile> UserProfiles { get; set; } = default!;

    private readonly IOptions<InitialDbSettings> initSettings_;
    private readonly UserProfileProvider userProfileProvider_;

    public AppDbContext(DbContextOptions options, IOptions<InitialDbSettings> initSettings, UserProfileProvider userProfileProvider) : base(options)
    {
        initSettings_ = initSettings;
        userProfileProvider_ = userProfileProvider;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var adminRole = new Role(initSettings_.Value.AdminName);
        var ownerRole = new Role(initSettings_.Value.OwnerName);

        var ownerUser = new User(initSettings_.Value.AdminName);
        var passwordHasher = new PasswordHasher<User>();
        ownerUser.PasswordHash = passwordHasher.HashPassword(ownerUser, initSettings_.Value.OwnerPassword);
        
        var ownerUserRole = new IdentityUserRole<string> { RoleId = ownerRole.Id, UserId = ownerUser.Id };
        var ownerProfile = userProfileProvider_.GetDefaultProfile(ownerUser.Id);

        builder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(p => p.UserId);

            entity.HasOne<User>()
                .WithOne()
                .HasForeignKey<UserProfile>(p => p.UserId);
        });

        builder.Entity<Role>().HasData(adminRole, ownerRole);
        builder.Entity<User>().HasData(ownerUser);
        builder.Entity<IdentityUserRole<string>>().HasData(ownerUserRole);
        builder.Entity<UserProfile>().HasData(ownerProfile);
    }
}

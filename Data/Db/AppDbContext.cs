﻿using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserControl.Services;

namespace Data.Db;

public class AppDbContext : IdentityDbContext
{
    public const string AdminName = "admin";
    public const string PrimeAdminName = "prime_admin";

    public DbSet<UserProfile> UserProfiles { get; set; }

    private readonly UserProfileProvider userProfileProvider_;

    public AppDbContext(DbContextOptions options, UserProfileProvider userProfileProvider) : base(options)
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

        builder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(p => p.UserId);

            entity.HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey<UserProfile>(p => p.UserId);
        });

        builder.Entity<IdentityRole>().HasData(adminRole);
        builder.Entity<IdentityRole>().HasData(primeAdminRole);
        builder.Entity<IdentityUser>().HasData(primeAdminUser);
        builder.Entity<IdentityUserRole<string>>().HasData(primeAdminUserRole);
        builder.Entity<UserProfile>().HasData(primeAdminProfile);
    }
}

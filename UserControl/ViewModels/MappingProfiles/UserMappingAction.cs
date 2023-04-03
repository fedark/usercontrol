using AutoMapper;
using Data.Models;
using Data.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UserControl.ViewModels.MappingProfiles;

public class UserMappingAction : IMappingAction<User, UserViewModel>
{
    private readonly UserManager<User> userManager_;

    public UserMappingAction(UserManager<User> userManager)
    {
        userManager_ = userManager;
    }

    public void Process(User user, UserViewModel viewUser, ResolutionContext _)
    {
        viewUser.IsAdmin = userManager_.IsInAdminRoleAsync(viewUser.Id).GetAwaiter().GetResult();

        var userWithNavigation = userManager_.Users.Where(u => u.Id == user.Id).Include(u => u.UserProfile).Single();
        var userProfile = userWithNavigation.UserProfile;

        viewUser.Picture = userProfile.Picture;
        viewUser.PictureType = userProfile.PictureType;
    }
}

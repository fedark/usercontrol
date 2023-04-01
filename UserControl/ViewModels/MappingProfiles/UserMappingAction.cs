using AutoMapper;
using Data.Db;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Identity;

namespace UserControl.ViewModels.MappingProfiles;

public class UserMappingAction : IMappingAction<User, UserViewModel>
{
    private readonly UserManager<User> userManager_;
    private readonly AppDbContext context_;

    public UserMappingAction(UserManager<User> userManager, AppDbContext context)
    {
        userManager_ = userManager;
        context_ = context;
    }

    public void Process(User user, UserViewModel viewUser, ResolutionContext _)
    {
        viewUser.IsAdmin = userManager_.IsInAdminRoleAsync(viewUser.Id).GetAwaiter().GetResult();

        context_.Entry(user).Reference(u => u.UserProfile).Load();
        var userProfile = user.UserProfile;

        viewUser.Picture = userProfile.Picture;
        viewUser.PictureType = userProfile.PictureType;
    }
}

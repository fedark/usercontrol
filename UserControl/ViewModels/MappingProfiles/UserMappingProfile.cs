using AutoMapper;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UserControl.ViewModels.MappingProfiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile(UserManager<User> userManager)
    {
        CreateMap<User, UserViewModel>()
            .AfterMap(async (_, viewUser) =>
            {
                viewUser.IsAdmin = await userManager.IsInAdminRoleAsync(viewUser.Id);

                await userManager.Users.Where(u => u.Id == viewUser.Id).LoadAsync();
                var userProfile = (await userManager.Users.SingleAsync(u => u.Id == viewUser.Id)).UserProfile;

                viewUser.Picture = userProfile.Picture;
                viewUser.PictureType = userProfile.PictureType;
            });
        
    }
}

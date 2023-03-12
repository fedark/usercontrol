using Microsoft.AspNetCore.Identity;
using UserControl.Models;

namespace UserControl.Services;

public class DefaultUserProfileProvider
{
    private readonly IWebHostEnvironment webHostEnvironment_;

    public DefaultUserProfileProvider(IWebHostEnvironment webHostEnvironment)
    {
        webHostEnvironment_ = webHostEnvironment;
    }

    public UserProfile GetDefaultProfile(IdentityUser user)
    {
        var defaultPicturePath = GetDefaultPicturePath();
        var defaultPictureBytes = File.ReadAllBytes(defaultPicturePath);

        return CreateUserProfile(user, defaultPictureBytes);
    }

    public async Task<UserProfile> GetDefaultProfileAsync(IdentityUser user)
    {
        var defaultPicturePath = GetDefaultPicturePath();
        var defaultPictureBytes = await File.ReadAllBytesAsync(defaultPicturePath);

        return CreateUserProfile(user, defaultPictureBytes);
    }

    private string GetDefaultPicturePath()
    {
        var webRoot = webHostEnvironment_.WebRootPath;
        return Path.Combine(webRoot, "images", "no_user_picture.png");
    }

    private UserProfile CreateUserProfile(IdentityUser user, byte[] pictureData)
    {
        return new UserProfile { UserId = user.Id, Picture = pictureData, PictureType = "image/png" };
    }
}

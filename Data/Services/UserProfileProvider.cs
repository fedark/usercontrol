using Data.Models;
using Microsoft.AspNetCore.Identity;

namespace UserControl.Services;

public class UserProfileProvider
{
    public UserProfileProvider()
    {
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
        var staticDataDir = "static_data";

        var contentDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.FullName,
            typeof(UserProfileProvider).Assembly.GetName().Name!,
            staticDataDir);

        if (!Directory.Exists(contentDir))
        {
            contentDir = Path.Combine(Environment.CurrentDirectory, staticDataDir);
        }

        return Path.Combine(contentDir, "no_user_picture.png");
    }

    private UserProfile CreateUserProfile(IdentityUser user, byte[] pictureData)
    {
        return new UserProfile { UserId = user.Id, Picture = pictureData, PictureType = "image/png" };
    }
}

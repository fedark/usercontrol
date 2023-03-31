using Data.Models;

namespace Data.Services;

public class UserProfileProvider
{
    public UserProfile GetDefaultProfile(string userId)
    {
        var defaultPicturePath = GetDefaultPicturePath();
        var defaultPictureBytes = File.ReadAllBytes(defaultPicturePath);

        return CreateUserProfile(userId, defaultPictureBytes);
    }

    public async Task<UserProfile> GetDefaultProfileAsync(string userId)
    {
        var defaultPicturePath = GetDefaultPicturePath();
        var defaultPictureBytes = await File.ReadAllBytesAsync(defaultPicturePath);

        return CreateUserProfile(userId, defaultPictureBytes);
    }

    private string GetDefaultPicturePath()
    {
        var staticDataDir = "static_data";
        var contentDir = Path.Combine(Environment.CurrentDirectory, staticDataDir);

        return Path.Combine(contentDir, "no_user_picture.png");
    }

    private UserProfile CreateUserProfile(string userId, byte[] pictureData)
    {
        return new() { UserId = userId, Picture = pictureData, PictureType = "image/png" };
    }
}

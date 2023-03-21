#nullable disable

namespace Data.Models;

public class UserProfile
{
    public string UserId { get; set; }
    public byte[] Picture { get; set; }
    public string PictureType { get; set; }
}

using Microsoft.AspNetCore.Identity;

namespace Data.Models;
public class User : IdentityUser
{
    public UserProfile UserProfile { get; set; } = default!;

    public User()
    {

    }

    public User(string userName) : base(userName)
    {
        NormalizedUserName = userName.ToUpper();
    }
}

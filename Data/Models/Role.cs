using Microsoft.AspNetCore.Identity;

namespace Data.Models;

public class Role : IdentityRole
{
    public const string Admin = "admin";
    public const string Owner = "owner";

    public Role()
    {

    }

    public Role(string roleName) : base(roleName)
    {
        NormalizedName = roleName.ToUpper();
    }
}

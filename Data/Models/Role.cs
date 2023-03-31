using Microsoft.AspNetCore.Identity;

namespace Data.Models;

public class Role : IdentityRole
{
    public Role(string roleName) : base(roleName)
    {
        NormalizedName = roleName.ToUpper();
    }
}

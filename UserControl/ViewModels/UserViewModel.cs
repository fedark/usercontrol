using System.ComponentModel.DataAnnotations;

namespace UserControl.ViewModels;

public class UserViewModel
{
    public string Id { get; set; } = default!;

    [Display(Name = "UserName")]
    public string UserName { get; set; } = default!;

    [Display(Name = "UserPicture")]
    public byte[]? Picture { get; set; } = default!;

    public string? PictureType { get; set; } = default!;

    [Display(Name = "Administrator")]
    public bool IsAdmin { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace UserControl.ViewModels;

public class UserViewModel
{
    public string Id { get; set; } = default!;

    [Display(Name = "User Name")]
    public string UserName { get; set; } = default!;

    [Display(Name = "User Picture")]
    public byte[]? Picture { get; set; } = default!;

    public string? PictureType { get; set; } = default!;

    [Display(Name = "Administrator")]
    public bool IsAdmin { get; set; }
}

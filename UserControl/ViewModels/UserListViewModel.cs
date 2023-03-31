namespace UserControl.ViewModels;

public record UserListViewModel(IEnumerable<UserViewModel> Users)
{
    public Dictionary<string, string> Headers { get; } = new()
    {
        [nameof(UserViewModel.UserName)] = "User Name",
        [nameof(UserViewModel.Picture)] = "User Picture",
        [nameof(UserViewModel.IsAdmin)] = "Administrator",
    };
}

namespace UserControl.Models
{
    public class DisplayUserModel
    {
#nullable disable
        public string Id { get; set; }
        public string UserName { get; set; }
#nullable restore
        public byte[]? Picture { get; set; }
        public string? PictureType { get; set; }
        public bool IsAdmin { get; set; }
    }
}

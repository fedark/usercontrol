namespace UserControl.Models
{
    public class DisplayUserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public byte[]? Picture { get; set; }
        public string? PictureType { get; set; }
        public bool IsAdmin { get; set; }
    }
}

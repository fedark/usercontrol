#nullable disable

namespace UserControl.Models
{
    public class UserProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public byte[] Picture { get; set; }
        public string PictureType { get; set; }
    }
}

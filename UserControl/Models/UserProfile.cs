namespace UserControl.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public byte[] Picture { get; set; }
        public string PictureType { get; set; }
    }
}

namespace CustomerHub.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        // Hash yok, sadece adı öyle; gerçek bi proje olsa hashing olurdu
        public string PasswordHash { get; set; }
        public string Email { get; set; }
    }
}

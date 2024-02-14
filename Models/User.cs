namespace CamplusBetaBackend.Models {
    public class User {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set;}
        public string Salt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }
}

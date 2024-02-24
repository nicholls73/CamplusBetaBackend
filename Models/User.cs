using Microsoft.AspNetCore.Identity;

namespace CamplusBetaBackend.Models {
    public class User : IdentityUser<Guid> {}

    public class LoginDetails {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}

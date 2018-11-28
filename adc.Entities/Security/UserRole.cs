using Microsoft.AspNetCore.Identity;

namespace adc.Entities.Security {
    public class UserRole : IdentityUserRole<long> {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}

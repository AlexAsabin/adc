using System.Linq;
using adc.Core.Interfaces;

namespace adc.Dtos.Security {
    public class UserModel : IDto, IId<long> {
        public long Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Initials => $"{FirstName?.FirstOrDefault()}{LastName?.FirstOrDefault()}".ToUpperInvariant();
        public string Photo { get; set; }
        public bool InActive { get; set; }
        public long? SiteId { get; set; }
        public bool IsAdmin { get; set; }

        public string Password { get; set; }
    }
}

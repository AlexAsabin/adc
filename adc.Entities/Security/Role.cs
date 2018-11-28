using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using adc.Core.Interfaces;

namespace adc.Entities.Security {
    public class Role : IdentityRole<long>, IId<long>, IEntity {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}

using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using adc.Core.Interfaces;
using adc.Entities.Core;

namespace adc.Entities.Security {
    public class User : IdentityUser<long>, IId<long>, IEntity {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Photo { get; set; }
        public bool InActive { get; set; }
        public long? SiteId { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<FileSystemEntry> FileSystemEntries { get; set; }
    }
}
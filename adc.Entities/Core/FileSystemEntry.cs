using adc.Core.Enums;
using adc.Core.Interfaces;
using adc.Entities.Security;

namespace adc.Entities.Core {
    public class FileSystemEntry : IId<long>, IEntity {
        public long Id { get; set; }
        public bool Ready { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public long CreateDate { get; set; }
        public long ChangeDate { get; set; }
        public long FileSize { get; set; }
        public FileSystemEntryType FileType { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
    }
}

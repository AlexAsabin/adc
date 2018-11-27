using adc.Core.Interfaces;

namespace adc.Entities {
    public class Conversion : IId<long>, IEntity {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}

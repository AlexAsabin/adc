using adc.Core.Interfaces;

namespace adc.Dtos {
    public class ConversionModel : IId<long>, IDto {

        public long Id { get; set; }
        public string Name { get; set; }

    }
}

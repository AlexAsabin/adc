﻿using adc.Core.Interfaces;

namespace adc.Entities {
    public class Conversions : IId<long>, IEntity {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}

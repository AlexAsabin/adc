using System;
using System.Collections.Generic;
using System.Text;

namespace adc.Core.Interfaces {
    public interface IId<T> {
        T Id { get; set; }
    }
}

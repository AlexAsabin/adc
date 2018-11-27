using adc.Dtos;
using adc.Entities;
using adc.Services.Base;
using adc.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace adc.Services.Implementation {
    public class ConversionService : BaseService<Conversion, ConversionModel>, IConversionService {
        public ConversionService(IServiceProvider serviceProvider) : base(serviceProvider) {

        }
    }
}

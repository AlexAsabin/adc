using System;
using AutoMapper;
using adc.Services.Implementation;
using adc.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace adc.Services {
    public static class ServicesServiceCollectionExtensions {
        public static IServiceCollection AddAdcServices(this IServiceCollection services) {

            services
                .AddAutoMapper()
                .AddServices();

            return services;
        }
        private static IServiceCollection AddServices(this IServiceCollection services) {
            services.AddScoped(typeof(IConversionService), typeof(ConversionService));
            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using adc.Dal.Implementation;
using adc.Dal.Interfaces;
using Start.Dal.Implementation;

namespace adc.Dal {
    public static class RepositotyServiceCollectionExtensions {
        public static IServiceCollection AddDbRepositories(this IServiceCollection services) {
            services.AddScoped(typeof(IRepository<>), typeof(DbRepository<>));
            return services;
        }

        public static IServiceCollection AddMockRepositories(this IServiceCollection services) {
            services.AddScoped(typeof(IRepository<>), typeof(MockRepository<>));
            return services;
        }
    }
}
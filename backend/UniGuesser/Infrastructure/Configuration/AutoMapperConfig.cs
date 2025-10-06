using Microsoft.Extensions.DependencyInjection;
using UniGuesser.Infrastructure;

namespace UniGuesser.Infrastructure.Configuration
{
    public static class AutoMapperConfig
    {
        public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddAutoMapper(typeof(AutoMapperConfig).Assembly);
            return services;
        }
    }
}

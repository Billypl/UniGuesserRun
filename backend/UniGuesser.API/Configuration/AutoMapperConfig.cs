using Microsoft.Extensions.DependencyInjection;

namespace UniGuesser.API.Configuration;

public static class AutoMapperConfig
{
    public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddAutoMapper(typeof(AutoMapperConfig).Assembly);
        return services;
    }
}
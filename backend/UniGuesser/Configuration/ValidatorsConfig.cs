using FluentValidation.AspNetCore;
using FluentValidation;
using Models.Validations;

namespace PartyGame.DependencyInjections
{
    public static class ValidatorsConfig
    {
        public static IServiceCollection AddValidatorsConfig(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<StartDataValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();
            return services;
        }
    }
}

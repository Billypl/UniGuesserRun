
using FluentValidation;
using FluentValidation.AspNetCore;
using UniGuesser.Application.Models.Validations;

namespace UniGuesser.Infrastructure.Configuration
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

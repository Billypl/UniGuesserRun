using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using UniGuesser.Application.Validations;

namespace UniGuesser.API.Configuration;

public static class ValidatorsConfig
{
    public static IServiceCollection AddValidatorsConfig(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation(); // This extension method is in FluentValidation.AspNetCore
        services.AddValidatorsFromAssemblyContaining<StartDataValidator>();
        services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();
        return services;
    }
}
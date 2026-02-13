using FluentValidation;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.Handlers;
using Soat.Eleven.Kutcut.Users.Application.Interfaces;
using Soat.Eleven.Kutcut.Users.Application.Validators;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Repositories;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Services;
using Soat.Eleven.Kutcut.Users.Infra.Repositories;
using Soat.Eleven.Kutcut.Users.Infra.Services;

namespace Soat.Eleven.Kutcut.Users.Api.Configurations;

public static class RegisterConfigurations
{
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<CreateUserInput>, CreateUserInputValidator>();
        services.AddTransient<IValidator<UpdateUserInput>, UpdateUserInputValidator>();
        services.AddTransient<IValidator<DeactiveUserInput>, DeactiveUserInputValidator>();
        services.AddTransient<IValidator<GetUserInput>, GetUserInputValidator>();
        services.AddTransient<IValidator<LoginInput>, LoginInputValidator>();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public static void AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<IUserHandler, UserHandler>();
        services.AddScoped<ILoginHandle, LoginHandle>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<IPasswordService, PasswordService>();
        services.AddTransient<IJwtTokenService, JwtTokenService>();
    }
}

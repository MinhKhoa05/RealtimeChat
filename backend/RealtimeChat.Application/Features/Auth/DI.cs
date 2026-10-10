using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddScoped<RegisterUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<LogoutUseCase>();
        services.AddScoped<LogoutAllUseCase>();
        services.AddScoped<ChangePasswordUseCase>();

        return services;
    }
}
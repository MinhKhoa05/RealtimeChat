using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Application.Features.Auth;
using RealtimeChat.Application.Features.Users;

namespace RealtimeChat.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Auths
        services.AddScoped<RegisterUseCase>();
        services.AddScoped<LoginUseCase>();

        return services;
    }
}
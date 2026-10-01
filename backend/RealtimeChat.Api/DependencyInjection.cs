using RealtimeChat.Application.Interfaces;
using RealtimeChat.Api.Hubs;
using RealtimeChat.Api.Security;

namespace RealtimeChat.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Security
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddJwtAuthentication(configuration);
        services.AddAuthorization();

        // SignalR
        services.AddSignalR();
        services.AddScoped<IClientNotifier, SignalRNotifier>();

        return services;
    }
}
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Api.Hubs;
using RealtimeChat.Api.Security;

namespace RealtimeChat.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(
        this IServiceCollection services)
    {
        // Security
        services.AddScoped<ICurrentUser, CurrentUser>();

        // SignalR
        services.AddSignalR();
        services.AddScoped<IChatNotifier, SignalRChatNotifier>();

        return services;
    }
}
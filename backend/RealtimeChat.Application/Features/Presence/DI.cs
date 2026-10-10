using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Presence;

public static class DependencyInjection
{
    public static IServiceCollection AddPresence(this IServiceCollection services)
    {
        services.AddScoped<IPresenceService, PresenceService>();

        return services;
    }
}
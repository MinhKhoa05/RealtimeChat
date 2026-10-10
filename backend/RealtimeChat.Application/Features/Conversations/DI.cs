using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Conversations;

public static class DependencyInjection
{
    public static IServiceCollection AddConversations(this IServiceCollection services)
    {
        services.AddScoped<StartDirectConversationUseCase>();
        services.AddScoped<GetConversationUseCase>();
        services.AddScoped<GetListConversationUseCase>();
        services.AddScoped<SetConversationPinUseCase>();

        return services;
    }
}
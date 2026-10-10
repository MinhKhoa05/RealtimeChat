using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Messages;

public static class DependencyInjection
{
    public static IServiceCollection AddMessages(this IServiceCollection services)
    {
        services.AddScoped<SendMessageUseCase>();
        services.AddScoped<RecallMessageUseCase>();
        services.AddScoped<GetMessagesUseCase>();

        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Application.Features.Messages.Services;

namespace RealtimeChat.Application.Features.Messages;

public static class DependencyInjection
{
    public static IServiceCollection AddMessages(this IServiceCollection services)
    {
        services.AddScoped<IMessageService, MessageService>();

        services.AddScoped<SendTextMessageUseCase>();
        services.AddScoped<SendMediaMessageUseCase>();
        services.AddScoped<SendStickerMessageUseCase>();
        services.AddScoped<RecallMessageUseCase>();
        services.AddScoped<GetMessagesUseCase>();

        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Application.Features.Calls.Services;

namespace RealtimeChat.Application.Features.Calls;

public static class DependencyInjection
{
    public static IServiceCollection AddCall(this IServiceCollection services)
    {
        services.AddScoped<ICallHistoryService, CallHistoryService>();

        services.AddScoped<CreateCallUseCase>();
        services.AddScoped<CancelledCallUseCase>();
        services.AddScoped<AcceptCallUseCase>();
        services.AddScoped<EndCallUseCase>();
        services.AddScoped<RejectedCallUseCase>();

        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Calls;

public static class DependencyInjection
{
    public static IServiceCollection AddCall(this IServiceCollection services)
    {
        services.AddScoped<CreateCallUseCase>();
        services.AddScoped<CancelledCallUseCase>();
        services.AddScoped<AcceptCallUseCase>();
        services.AddScoped<EndCallUseCase>();
        services.AddScoped<RejectedCallUseCase>();

        return services;
    }
}
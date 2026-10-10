using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Media;

public static class DependencyInjection
{
    public static IServiceCollection AddMedia(this IServiceCollection services)
    {
        services.AddScoped<UploadMediaUseCase>();

        return services;
    }
}
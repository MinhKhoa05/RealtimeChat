using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.ShareTrip;

public static class DependencyInjection
{
    public static IServiceCollection AddShareTrip(this IServiceCollection services)
    {
        services.AddScoped<CreateShareTripUseCase>();
        services.AddScoped<EndShareTripUseCase>();
        services.AddScoped<UpdateUserLiveLocationUseCase>();

        services.AddScoped<GetConversationShareTripsUseCase>();
        services.AddScoped<GetShareTripByIdUseCase>();
        services.AddScoped<GetUserShareTripsUseCase>();

        return services;
    }
}
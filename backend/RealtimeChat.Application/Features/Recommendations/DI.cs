using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Application.Features.Recommendations.CandidatePool;

namespace RealtimeChat.Application.Features.Recommendations;

public static class DependencyInjection
{
    public static IServiceCollection AddRecommendations(this IServiceCollection services)
    {
        services.AddScoped<IPoolGenerator, PoolGenerator>();
        services.AddScoped<IPoolManager, PoolManager>();

        services.AddScoped<GetFriendRecommendationUseCase>();
        return services;
    }
}
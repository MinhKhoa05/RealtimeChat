using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Application.Features.Recommendations.CandidatePool;
using RealtimeChat.Application.Features.Recommendations.Services;

namespace RealtimeChat.Application.Features.Recommendations;

public static class DependencyInjection
{
    public static IServiceCollection AddRecommendations(this IServiceCollection services)
    {
        services.AddScoped<IPoolCache, PoolCache>();
        services.AddScoped<IPoolGenerator, PoolGenerator>();
        services.AddScoped<IFriendRecommendationService, FriendRecommendationService>();
        services.AddScoped<GetFriendRecommendationUseCase>();

        return services;
    }
}
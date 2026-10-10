using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Friends;

public static class DependencyInjection
{
    public static IServiceCollection AddFriends(this IServiceCollection services)
    {
        services.AddScoped<SendFriendRequestUseCase>();
        services.AddScoped<AcceptFriendRequestUseCase>();
        services.AddScoped<RejectFriendRequestUseCase>();
        services.AddScoped<RevokeFriendRequestUseCase>();
        services.AddScoped<RemoveFriendUseCase>();
        services.AddScoped<GetFriendsUseCase>();
        services.AddScoped<GetSentFriendRequestsUseCase>();
        services.AddScoped<GetReceivedFriendRequestsUseCase>();

        return services;
    }
}
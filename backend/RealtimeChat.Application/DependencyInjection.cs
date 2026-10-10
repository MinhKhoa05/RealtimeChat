using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Application.Features.Auth;
using RealtimeChat.Application.Features.Blocks;
using RealtimeChat.Application.Features.Friends;
using RealtimeChat.Application.Features.Users;
using RealtimeChat.Application.Features.Groups;
using RealtimeChat.Application.Features.Conversations;
using RealtimeChat.Application.Features.Presence;
using RealtimeChat.Application.Features.Messages;
using RealtimeChat.Application.Features.Recommendations;
using RealtimeChat.Application.Features.Media;
using RealtimeChat.Application.Features.Stickers;
using RealtimeChat.Application.Features.ShareTrip;
using RealtimeChat.Application.Features.Call;

namespace RealtimeChat.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAuth();
        services.AddBlocks();
        services.AddCall();
        services.AddConversations();
        services.AddFriends();
        services.AddGroups();
        services.AddMedia();
        services.AddMessages();
        services.AddPresence();
        services.AddRecommendations();
        services.AddShareTrip();
        services.AddStickers();
        services.AddUsers();

        return services;
    }
}
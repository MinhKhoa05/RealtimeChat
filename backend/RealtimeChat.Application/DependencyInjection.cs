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
using RealtimeChat.Application.Features.Recommendations.CandidatePool;
using RealtimeChat.Application.Features.Recommendations.Services;
using RealtimeChat.Application.Features.Media;

namespace RealtimeChat.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Auths
        services.AddScoped<RegisterUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<LogoutUseCase>();
        services.AddScoped<LogoutAllUseCase>();
        services.AddScoped<ChangePasswordUseCase>();

        // Users
        services.AddScoped<GetCurrentUserUseCase>();
        services.AddScoped<GetUserProfileUseCase>();
        services.AddScoped<SearchUsersUseCase>();

        // Friends
        services.AddScoped<SendFriendRequestUseCase>();
        services.AddScoped<AcceptFriendRequestUseCase>();
        services.AddScoped<RejectFriendRequestUseCase>();
        services.AddScoped<RevokeFriendRequestUseCase>();
        services.AddScoped<RemoveFriendUseCase>();
        services.AddScoped<GetFriendsUseCase>();
        services.AddScoped<GetSentFriendRequestsUseCase>();
        services.AddScoped<GetReceivedFriendRequestsUseCase>();

        // Blocks
        services.AddScoped<BlockUserUseCase>();
        services.AddScoped<UnblockUserUseCase>();
        services.AddScoped<GetBlockedUsersUseCase>();

        // Groups
        services.AddScoped<AddMemberUseCase>();
        services.AddScoped<CreateGroupUseCase>();
        services.AddScoped<GetGroupMembersUseCase>();
        services.AddScoped<GetGroupUseCase>();
        services.AddScoped<GetMyGroupsUseCase>();
        services.AddScoped<KickMemberUseCase>();
        services.AddScoped<LeaveGroupUseCase>();
        services.AddScoped<TransferAdminUseCase>();
        services.AddScoped<DisbandGroupUseCase>();

        // Conversations
        services.AddScoped<StartDirectConversationUseCase>();
        services.AddScoped<GetConversationUseCase>();
        services.AddScoped<GetListConversationUseCase>();
        services.AddScoped<SetConversationPinUseCase>();

        // Presence
        services.AddScoped<IPresenceService, PresenceService>();

        // Messages
        services.AddScoped<SendMessageUseCase>();
        services.AddScoped<RecallMessageUseCase>();
        services.AddScoped<GetMessagesUseCase>();

        // Recommendations
        services.AddScoped<IPoolCache, PoolCache>();
        services.AddScoped<IPoolGenerator, PoolGenerator>();
        services.AddScoped<IFriendRecommendationService, FriendRecommendationService>();
        services.AddScoped<GetFriendRecommendationUseCase>();

        // Media
        services.AddScoped<UploadMediaUseCase>();

        return services;
    }
}
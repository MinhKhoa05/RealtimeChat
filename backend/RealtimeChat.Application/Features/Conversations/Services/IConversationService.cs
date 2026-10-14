using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Conversations.Services;

public interface IConversationService
{
    Task<Conversation> GetOrCreateDirectConversationAsync(long userId, long otherUserId, CancellationToken ct);
}
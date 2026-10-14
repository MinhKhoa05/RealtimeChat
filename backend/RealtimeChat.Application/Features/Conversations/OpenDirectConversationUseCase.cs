using RealtimeChat.Application.Features.Conversations.Services;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Conversations;

public class OpenDirectConversationUseCase
{
    private readonly ICurrentUser _currentUser;
    private readonly IConversationService _conversationService;

    public OpenDirectConversationUseCase(IConversationService conversationService, ICurrentUser currentUser)
    {
        _currentUser = currentUser;
        _conversationService = conversationService;
    }

    public async Task<long> ExecuteAsync(long otherUserId, CancellationToken ct)
    {
        var conversation = await _conversationService.GetOrCreateDirectConversationAsync(_currentUser.UserId, otherUserId, ct);
        return conversation.Id;
    }
}
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Conversations;

public class GetConversationUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetConversationUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ConversationResponse> ExecuteAsync(long conversationId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var conversation = await _context.Conversations
            .AsNoTracking()
            .Active()
            .GetById(conversationId)
            .AccessibleBy(currentUserId)
            .Select(x => new ConversationResponse
            {
                Id = x.Id,
                Type = x.Type,
                AvatarUrl = null,

                Name = x.Type == ConversationType.Direct
                    ? x.Members.First(m => m.MemberId != currentUserId).Member.Name
                    : x.Name,

                OtherUserId = x.Type == ConversationType.Direct
                    ? x.Members.First(m => m.MemberId != currentUserId).MemberId
                    : null,
            })
            .FirstOrDefaultAsync(ct);

        if (conversation is null)
        {
            throw new Exception("Conversation not found or forbidden");
        }

        return conversation;
    }
}

public class ConversationResponse
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public ConversationType Type { get; set; }

    public string? AvatarUrl { get; set; }

    public long? OtherUserId { get; set; } // Type == Direct
}
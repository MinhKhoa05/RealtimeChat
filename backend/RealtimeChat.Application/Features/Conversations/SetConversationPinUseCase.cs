using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Conversations;

public class SetConversationPinUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public SetConversationPinUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long conversationId, SetConversationPinRequest request, CancellationToken ct)
    {
        var member = await _context.ConversationMembers
            .FirstOrDefaultAsync(x =>
                x.ConversationId == conversationId &&
                x.MemberId == _currentUser.UserId &&
                x.Conversation.DisbandedAt == null,
                ct)
            ?? throw new NotFoundException();

        if (request.IsPinned)
        {
            member.Pin();
        }
        else
        {
            member.Unpin();
        }

        await _context.SaveChangesAsync(ct);
    }
}

public class SetConversationPinRequest
{
    public bool IsPinned { get; set; }
}
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

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
        var userId = _currentUser.UserId;

        var conversation = await _context.Conversations
            .Where(x => x.Id == conversationId)
            .IncludeMembers(userId)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        conversation.SetPinned(userId, request.IsPinned);
        await _context.SaveChangesAsync(ct);
    }
}

public class SetConversationPinRequest
{
    public bool IsPinned { get; set; }
}
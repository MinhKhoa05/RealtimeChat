using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Application.Common.Pagination;
using RealtimeChat.Application.Exceptions;

namespace RealtimeChat.Application.Features.Messages;

public class GetMessagesUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMessagesUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CursorPaginationResult<MessageResponse>> ExecuteAsync(long conversationId, CursorPaginationQuery request, CancellationToken ct)
    {
        var canAccessConversation = await _context.Conversations
            .FilterAccessible(conversationId, _currentUser.UserId)
            .AnyAsync(ct);

        if (!canAccessConversation)
        {
            throw new ForbiddenException();
        }

        var result = await _context.Messages
            .AsNoTracking()
            .Where(x => x.ConversationId == conversationId)
            .Include(x => x.Media)
            .Include(x => x.Call)
            .Include(x => x.Sticker)
            .Include(x => x.Mentions)
            .ToCursorPageAsync(request, ct);

        return result.Map(MessageMapper.ToResponse);
    }
}
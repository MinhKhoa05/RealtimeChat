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
        var isMember = await _context.Conversations.IsMemberAsync(conversationId, _currentUser.UserId, ct);
        if (!isMember)
        {
            throw new ForbiddenException();
        }
        
        var messages = await _context.Messages
            .AsNoTracking()
            .Where(x => x.ConversationId == conversationId)
            .Include(x => x.Media)
            .Include(x => x.Call)
            .Include(x => x.Sticker)
            .Include(x => x.Mentions)
            .Include(x => x.ReplyToMessage) 
                .ThenInclude(x => x!.Mentions)
            .ToCursorPageAsync(request, ct);

        return messages.Map(MessageMapper.ToResponse);
    }
}
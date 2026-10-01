using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Conversations;

public class StartDirectConversationUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public StartDirectConversationUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<long> ExecuteAsync(StartDirectConversationRequest request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;
        var userId = request.UserId;

        var conversation = await _context.Conversations
            .Direct(currentUserId, userId)
            .Active()
            .FirstOrDefaultAsync(ct);

        if (conversation is not null) return conversation.Id;

        var newConversation = Conversation.CreateDirect(currentUserId, userId);

        _context.Conversations.Add(newConversation);

        try
        {
            await _context.SaveChangesAsync(ct);
            return newConversation.Id;
        }
        catch (DuplicateKeyException)
        {
            return await _context.Conversations
                .Direct(currentUserId, userId)
                .Active()
                .Select(x => x.Id)
                .FirstAsync(ct);
        }
    }
}

public class StartDirectConversationRequest
{
    public long UserId {get; set; }
}
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.ShareTrip;

public class GetShareTripByIdUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetShareTripByIdUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ShareTripSession> ExecuteAsync(long shareTripId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var session = await _context.ShareTripSessions.FindAsync(shareTripId, ct)
            ?? throw new NotFoundException();

        var canAccessConversation = await _context.Conversations
            .FilterAccessible(session.ConversationId, currentUserId)
            .AnyAsync(ct);
        
        if (!canAccessConversation)
        {
            throw new ForbiddenException();
        }

        return session;
    }
}
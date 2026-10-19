using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.ShareTrip;

public class GetConversationShareTripsUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetConversationShareTripsUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ShareTripSession>> ExecuteAsync(long conversationId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var isMember = await _context.Conversations.IsMemberAsync(conversationId, currentUserId, ct);
        if (!isMember)
        {
            throw new ForbiddenException();
        }

        var trips = await _context.ShareTripSessions
            .Where(x => x.ConversationId == conversationId)
            .Where(x => x.Status == Domain.Enums.ShareTripStatus.Active)
            .ToListAsync(ct);
        
        return trips;
    }
}
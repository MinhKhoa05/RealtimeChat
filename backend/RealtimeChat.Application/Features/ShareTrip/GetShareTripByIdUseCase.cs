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

        var trip = await _context.ShareTripSessions.FindAsync(shareTripId, ct)
            ?? throw new NotFoundException();

        var isMember = await _context.Conversations.IsMemberAsync(trip.ConversationId, currentUserId, ct);
        if (!isMember)
        {
            throw new ForbiddenException();
        }

        return trip;
    }
}
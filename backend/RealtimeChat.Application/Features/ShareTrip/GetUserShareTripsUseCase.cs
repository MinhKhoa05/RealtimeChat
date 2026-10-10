using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.ShareTrip;

public class GetUserShareTripsUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetUserShareTripsUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ShareTripSession>> ExecuteAsync(CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var sessions = await _context.ShareTripSessions
            .Where(x => x.OwnerId == currentUserId)
            .Where(x => x.Status == Domain.Enums.ShareTripStatus.Active)
            .ToListAsync(ct);

        return sessions;
    }
}
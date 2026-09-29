using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Friends;

public class RejectFriendRequestUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public RejectFriendRequestUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long requestId, CancellationToken ct)
    {
        var friendRequest = await _context.FriendRequests.FindAsync(requestId)
            ?? throw new Exception("Request not found");

        if (_currentUser.UserId != friendRequest.ReceiverId)
        {
            throw new Exception("Cannot reject");
        }

        _context.FriendRequests.Remove(friendRequest);
        await _context.SaveChangesAsync(ct);
    }
}
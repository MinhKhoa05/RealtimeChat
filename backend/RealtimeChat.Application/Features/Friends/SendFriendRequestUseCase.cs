using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Friends;

public class SendFriendRequestUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public SendFriendRequestUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(SendFriendRequestRequest request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        if (currentUserId == request.UserId)
        {
            throw new Exception("Cannot send friend request to yourself.");
        }

        var targetExists = await _context.Users.AnyAsync(x => x.Id == request.UserId, ct);
        if (!targetExists)
            throw new Exception("User not found.");

        var user1Id = Math.Min(request.UserId, currentUserId);
        var user2Id = Math.Max(request.UserId, currentUserId);

        var isFriend = await _context.Friendships.AnyAsync(x => x.UserId == user1Id && x.FriendId == user2Id, ct);
        if (isFriend)
        {
            throw new Exception("Already friends.");
        }

        var existingRequest = await _context.FriendRequests
            .AnyAsync(x => x.SenderId == currentUserId && x.ReceiverId == request.UserId, ct);

        if (existingRequest)
        {
            throw new Exception("Friend request already sent");
        }

        var reverseRequest = await _context.FriendRequests
            .AnyAsync(x => x.SenderId == request.UserId && x.ReceiverId == currentUserId, ct);

        if (reverseRequest)
            throw new Exception("Friend request already received.");

        var friendRequest = new FriendRequest
        {
            SenderId = currentUserId,
            ReceiverId = request.UserId,
            Introduction = request.Introduction
        };
        
        _context.FriendRequests.Add(friendRequest);
        await _context.SaveChangesAsync(ct);
    }
}

public class SendFriendRequestRequest
{
    public long UserId { get; set; }
    public string? Introduction { get; set; }
}
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
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
        {
            throw new Exception("User not found.");
        }

        var isFriend = await _context.Relationships
            .Friend(currentUserId, request.UserId)
            .AnyAsync(ct);

        if (isFriend)
        {
            throw new Exception("Already friends.");
        }

        var existingRequest = await _context.Relationships
            .FriendRequests()
            .Between(currentUserId, request.UserId)
            .AnyAsync(ct);

        if (existingRequest)
        {
            throw new Exception("Friend request already exists.");
        }

        var friendRequest = Relationship.CreateFriendRequest(currentUserId, request.UserId, request.Introduction);

        _context.Relationships.Add(friendRequest);
        await _context.SaveChangesAsync(ct);
    }
}

public class SendFriendRequestRequest
{
    public long UserId { get; set; }
    public string? Introduction { get; set; }
}
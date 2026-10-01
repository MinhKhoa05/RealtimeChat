using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
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
        var targetUserId = request.UserId;

        if (currentUserId == targetUserId)
        {
            throw new BadRequestException("Cannot send friend request to yourself.");
        }

        var targetExists = await _context.Users.AnyAsync(x => x.Id == targetUserId, ct);

        if (!targetExists)
        {
            throw new NotFoundException();
        }

        var isFriend = await _context.Relationships
            .Friends()
            .Between(currentUserId, targetUserId)
            .AnyAsync(ct);

        if (isFriend)
        {
            throw new ConflictException("Already friends.");
        }

        var existingRequest = await _context.Relationships
            .FriendRequests()
            .Between(currentUserId, request.UserId)
            .AnyAsync(ct);

        if (existingRequest)
        {
            throw new ConflictException("Friend request already exists.");
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
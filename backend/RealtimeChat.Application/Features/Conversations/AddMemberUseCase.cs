using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Conversations;

public class AddMemberUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public AddMemberUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long conversationId, long userId, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        if (userId == actorId)
            throw new BadRequestException("Cannot add yourself.");

        var isFriend = await _context.Relationships
            .Friends()
            .Between(userId, actorId)
            .AnyAsync(ct);
        
        if (!isFriend)
            throw new ForbiddenException("Users must be friends");

        var conversation = await _context.Conversations.GetWithMembersAsync(conversationId, [actorId, userId], ct)
            ?? throw new NotFoundException();

        conversation.AddMember(actorId, userId);
        await _context.SaveChangesAsync(ct);
    }
}
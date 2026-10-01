using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Groups;

public class AddMemberUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public AddMemberUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long groupId, long userId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        if (userId == currentUserId)
        {
            throw new BadRequestException("Cannot add yourself.");
        }

        var isFriend = await _context.Relationships
            .Friends()
            .Between(userId, currentUserId)
            .AnyAsync(ct);
        
        if (!isFriend)
        {
            throw new ForbiddenException("Users must be friends");
        }

        var group = await _context.Conversations
            .FilterAccessibleGroup(groupId, currentUserId)
            .WithMembers(userId) // Lấy member để kiểm tra đã tham gia nhóm chưa
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        group.AddMember(userId);
        await _context.SaveChangesAsync(ct);
    }
}
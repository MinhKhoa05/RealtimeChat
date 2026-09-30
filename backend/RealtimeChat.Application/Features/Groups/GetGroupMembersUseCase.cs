using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Groups;

public class GetGroupMembersUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetGroupMembersUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<GroupMemberResponse>> ExecuteAsync(long groupId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var isMember = await _context.Conversations
            .Group(groupId)
            .AnyAsync(x => x.Members.Any(m => m.MemberId == currentUserId), ct);

        if (!isMember)
        {
            throw new Exception("Group not found or not member");
        }

        var members = await _context.ConversationMembers
            .AsNoTracking()
            .Where(x => x.ConversationId == groupId)
            .Select(x => new GroupMemberResponse
            {
                MemberId = x.MemberId,
                Name = x.Member.Name,
                AvatarUrl = null,
                Role = x.Role,
                JoinedAt = x.JoinedAt,
            })
            .ToListAsync(ct);

        return members;
    }
}

public class GroupMemberResponse
{
    public long MemberId { get; set; }
    public string Name { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public ConversationMemberRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
}
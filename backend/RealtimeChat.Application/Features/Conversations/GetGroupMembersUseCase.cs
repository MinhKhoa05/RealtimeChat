using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Conversations;

public class GetGroupMembersUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetGroupMembersUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<GroupMemberResponse>> ExecuteAsync(long conversationId, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        var members = await _context.Conversations
            .Where(x => x.Id == conversationId)
            .SelectMany(x => x.Members)
            .Select(x => new GroupMemberResponse
            {
                MemberId = x.MemberId,
                Role = x.Role,
                InvitedById = x.InvitedById,
                JoinedAt = x.CreatedAt,
            })
            .ToListAsync(ct);

        var hasAccess = members.Any(x => x.MemberId == actorId);
        if (!hasAccess)
        {
            throw new NotFoundException();
        }

        return members;
    }
}

public class GroupMemberResponse
{
    public long MemberId { get; set; }
    public ConversationMemberRole Role { get; set; }
    public long? InvitedById { get; set; }
    public DateTime JoinedAt { get; set; }
}
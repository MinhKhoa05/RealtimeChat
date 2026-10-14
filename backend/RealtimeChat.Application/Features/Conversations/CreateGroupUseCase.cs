using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Conversations;

public class CreateGroupUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CreateGroupUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<long> ExecuteAsync(CreateGroupRequest request, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        var memberIds = request.MemberIds.Where(x => x != actorId).Distinct().ToList();
        if (memberIds.Count < 2)
        {
            throw new BadRequestException("A group must have at least 3 members.");
        }

        var friendCount = await _context.Relationships
            .Friends()
            .BetweenUsers(actorId, memberIds)
            .CountAsync(ct);

        if (friendCount != memberIds.Count)
        {
            throw new ForbiddenException("All members must be friends.");
        }

        var conversation = Conversation.CreateGroup(actorId, request.Name);
        foreach (var memberId in memberIds)
        {
            conversation.AddMember(actorId, memberId);
        }

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync(ct);

        return conversation.Id;
    }
}

public class CreateGroupRequest
{
    public string Name { get; set; } = null!;
    public List<long> MemberIds {get; set; } = [];
}
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Groups;

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
        var currentUserId = _currentUser.UserId;

        var memberIds = request.MemberIds.Where(x => x != currentUserId).Distinct().ToList();
        if (memberIds.Count < 2)
        {
            throw new Exception("Group Least 3 member include yourself");
        }

        var friendCount = await _context.Relationships
            .Friends()
            .WithUsers(currentUserId, memberIds)
            .CountAsync(ct);

        if (friendCount != memberIds.Count)
        {
            throw new Exception("All members must be friends.");
        }

        var conversation = Conversation.CreateGroup(request.Name, currentUserId);
        foreach (var memberId in memberIds)
        {
            conversation.AddMember(memberId);
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
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Conversations;

public class LeaveGroupUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public LeaveGroupUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long groupId, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        var group = await _context.Conversations
            .FilterAccessibleGroup(groupId, actorId)
            .WithMembers(actorId) // Lấy member để thực hiện hành động ở Domain
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();
        
        group.Leave(actorId);
        await _context.SaveChangesAsync(ct);
    }
}
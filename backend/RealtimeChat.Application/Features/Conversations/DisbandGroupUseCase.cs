using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Conversations;

public class DisbandGroupUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;

    public DisbandGroupUseCase(IAppDbContext context, ICurrentUser currentUser, TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(long groupId, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        var group = await _context.Conversations
            .FilterAccessibleGroup(groupId, actorId)
            .WithMembers(actorId) // Lấy member để kiểm tra quyền
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        group.DisbandGroup(actorId, now);

        await _context.SaveChangesAsync(ct);
    }
}
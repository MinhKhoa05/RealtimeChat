using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Conversations;

public class JoinGroupByInviteUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public JoinGroupByInviteUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(Guid inviteCode, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var conversation = await _context.Conversations
            .Where(x => x.InviteCode == inviteCode)
            .Active()
            .WithMembers(currentUserId) // Load Members để Domain kiểm tra thành viên đã tồn tại.
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        conversation.AddMember(currentUserId);
        await _context.SaveChangesAsync(ct);
    }
}
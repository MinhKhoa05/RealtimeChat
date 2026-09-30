using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Groups;

public class TransferAdminUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public TransferAdminUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long groupId, TransferAdminRequest request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;
        var newAdminId = request.NewAdminId;

        var group = await _context.Conversations
            .Active()
            .Group(groupId)
            .Include(x => x.Members
                .Where(m => m.MemberId == currentUserId || m.MemberId == newAdminId))
            .FirstOrDefaultAsync(ct);
        
        if (group is null)
        {
            throw new Exception("Group not found");
        }

        group.TransferAdmin(currentUserId, newAdminId);
        await _context.SaveChangesAsync(ct);
    }
}

public class TransferAdminRequest
{
    public long NewAdminId { get; set; }
}
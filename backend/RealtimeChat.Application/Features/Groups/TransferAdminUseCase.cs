using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
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
            .FilterAccessibleGroup(groupId, currentUserId)
            .WithMembers(currentUserId, newAdminId) // Lấy member liên quan để thực hiện action
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        if (!group.IsAdmin(currentUserId))
        {
            throw new ForbiddenException();
        }

        group.TransferAdmin(newAdminId);
        await _context.SaveChangesAsync(ct);
    }
}

public class TransferAdminRequest
{
    public long NewAdminId { get; set; }
}
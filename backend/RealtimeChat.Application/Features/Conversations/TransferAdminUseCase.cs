using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Factories;

namespace RealtimeChat.Application.Features.Conversations;

public class TransferAdminUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public TransferAdminUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long conversationId, TransferAdminRequest request, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;
        var newAdminId = request.NewAdminId;

        var conversation = await _context.Conversations.GetWithMembersAsync(conversationId, [actorId, newAdminId], ct)
            ?? throw new NotFoundException();

        conversation.TransferAdmin(actorId, newAdminId);

        var message = SystemMessageFactory.CreateAdminTransferred(conversationId, actorId, newAdminId);
        _context.Messages.Add(message);

        await _context.SaveChangesAsync(ct);
    }
}

public class TransferAdminRequest
{
    public long NewAdminId { get; set; }
}
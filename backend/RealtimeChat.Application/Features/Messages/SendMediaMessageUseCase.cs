using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Features.Messages.Services;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Messages;

public class SendMediaMessageUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IMessageService _messageService;

    public SendMediaMessageUseCase(IAppDbContext context, ICurrentUser currentUser, IMessageService messageService)
    {
        _context = context;
        _currentUser = currentUser;
        _messageService = messageService;
    }

    public async Task ExecuteAsync(long conversationId, SendMediaMessageRequest request, CancellationToken ct)
    {
        var senderId = _currentUser.UserId;

        await _messageService.ValidateCanSendMessageAsync(conversationId, senderId, ct);

        // Kiểm tra Media tồn tại
        var media = await _context.Medias.FirstOrDefaultAsync(x => x.PublicId == request.MediaPublicId, ct)
            ?? throw new NotFoundException();

        // Lấy ReplyMessage nếu request có
        Message? replyToMessage = null;
        if (request.ReplyToMessageId.HasValue)
        {
            replyToMessage = await _context.Messages.FindAsync(request.ReplyToMessageId, ct)
                ?? throw new NotFoundException();
        }

        var message = Message.CreateMedia(conversationId, senderId, media);

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(ct);

        await _messageService.NotifyMessageCreatedAsync(message, ct);
    }
}

public class SendMediaMessageRequest
{
    public Guid MediaPublicId { get; set; }
    public long? ReplyToMessageId { get; set; }
}
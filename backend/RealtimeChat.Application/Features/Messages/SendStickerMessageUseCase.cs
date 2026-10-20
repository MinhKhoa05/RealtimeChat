using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Features.Messages.Services;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Messages;

public class SendStickerMessageUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IMessageService _messageService;

    public SendStickerMessageUseCase(IAppDbContext context, ICurrentUser currentUser, IMessageService messageService)
    {
        _context = context;
        _currentUser = currentUser;
        _messageService = messageService;
    }

    public async Task ExecuteAsync(long conversationId, SendStickerMessageRequest request, CancellationToken ct)
    {
        var senderId = _currentUser.UserId;

        await _messageService.ValidateCanSendMessageAsync(conversationId, senderId, ct);

        // Kiểm tra Sticker tồn tại
        var sticker = await _context.Stickers.FirstOrDefaultAsync(x => x.PublicId == request.StickerPublicId, ct)
            ?? throw new NotFoundException();

        var message = Message.CreateSticker(conversationId, senderId, sticker);

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(ct);

        await _messageService.NotifyMessageCreatedAsync(message, ct);
    }
}

public class SendStickerMessageRequest
{
    public Guid StickerPublicId { get; set; }
}
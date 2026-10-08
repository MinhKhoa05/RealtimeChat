using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Messages;

public class SendMessageUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;

    public SendMessageUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(long conversationId, SendMessageRequest request, CancellationToken ct)
    {
        var senderId = _currentUser.UserId;

        var canAccessConversation = await _context.Conversations
            .FilterAccessible(conversationId, senderId)
            .AnyAsync(ct);

        if (!canAccessConversation)
        {
            throw new ForbiddenException();
        }

        var message = await CreateMessage(conversationId, senderId, request, ct);

        if (request.Mentions.Count != 0)
        {
            await AddValidMentions(message, request.Mentions, ct);
        }

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(ct);

        var receiverIds = await _context.ConversationMembers
            .Where(x => x.ConversationId == conversationId)
            .Select(x => x.MemberId)
            .ToListAsync(ct);

        var response = MessageMapper.ToResponse(message);
        await _notifier.NotifyAsync(receiverIds, "message.created", response, ct);
    }

    // Factory tạo message
    private async Task<Message> CreateMessage(long conversationId, long senderId, SendMessageRequest request, CancellationToken ct)
    {
        var type = request.MessageType;

        if (type == MessageType.Text)
        {
            return Message.CreateText(conversationId, senderId, request.Content!);
        }

        if (type == MessageType.Sticker)
        {
            var sticker = await _context.Stickers
                .Include(x => x.Media)
                .FirstOrDefaultAsync(x => x.PublicId == request.ReferenceId, ct)
                ?? throw new NotFoundException("Sticker not found");

            return Message.CreateSticker(conversationId, senderId, sticker);
        }

        if (Message.IsMediaType(type))
        {
            var media = await _context.Medias.FirstOrDefaultAsync(x => x.PublicId == request.ReferenceId, ct)
                ?? throw new NotFoundException("Media not found");

            return Message.CreateMedia(conversationId, senderId, media, request.MessageType);
        }

        throw new BadRequestException($"Message type '{type}' is not supported.");
    }

    private async Task AddValidMentions(Message message, IReadOnlyCollection<MentionDto> mentions, CancellationToken ct)
    {
        if (message.Content is null)
            return;

        var userIds = mentions.Select(x => x.UserId).Distinct().ToList();

        var users = await _context.ConversationMembers
            .Where(x => x.ConversationId == message.ConversationId && userIds.Contains(x.MemberId))
            .Select(x => new
            {
                x.Member.Id,
                x.Member.Name
            })
            .ToDictionaryAsync(x => x.Id, ct);

        // Chỉ tạo Mention khi dữ liệu hợp lệ.
        // Nếu Mention không hợp lệ, bỏ qua và vẫn lưu Message bình thường.
        foreach (var mention in mentions)
        {
            if (!users.TryGetValue(mention.UserId, out var user))
                continue;

            if (mention.Start < 0 || mention.Length <= 0 || mention.Start > message.Content!.Length - mention.Length)
                continue;

            var mentionedText = message.Content.Substring(mention.Start, mention.Length);

            if (mentionedText != $"@{user.Name}")
                continue;

            message.AddMention(mention.UserId, mention.Start, mention.Length);
        }
    }
}

public class SendMessageRequest
{
    public string? Content { get; set; }
    public MessageType MessageType { get; set; }
    public Guid? ReferenceId { get; set; } // Dùng cho loại Sticker / Media
    public List<MentionDto> Mentions { get; set; } = [];
}

public class MentionDto
{
    public long UserId { get; init; }
    public int Start { get; init; }
    public int Length { get; init; }
}
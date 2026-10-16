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
            Message? replyToMessage = null;
            if (request.ReplyToMessageId.HasValue)
            {
                replyToMessage = await _context.Messages.FindAsync(request.ReplyToMessageId, ct)
                    ?? throw new NotFoundException();
            }

            var mentions = await GetValidMentions(conversationId, request.Content, request.Mentions, ct);
            return Message.CreateText(conversationId, senderId, request.Content!, mentions, replyToMessage);
        }

        if (type == MessageType.Media)
        {
            var media = await _context.Medias.FirstOrDefaultAsync(x => x.PublicId == request.ReferenceId, ct)
                ?? throw new NotFoundException();

            return Message.CreateMedia(conversationId, senderId, media);
        }

        if (type == MessageType.Sticker)
        {
            var sticker = await _context.Stickers.FirstOrDefaultAsync(x => x.PublicId == request.ReferenceId, ct)
                ?? throw new NotFoundException();

            return Message.CreateSticker(conversationId, senderId, sticker);
        }

        throw new BadRequestException($"Cannot support for message type {type}");
    }

    private async Task<ICollection<Mention>> GetValidMentions(long conversationId, string? content, ICollection<MentionDto> mentions, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(content))
            return [];

        var validMentions = new List<Mention>();

        var userIds = mentions.Select(x => x.UserId).Distinct().ToList();

        var users = await _context.ConversationMembers
            .Where(x => x.ConversationId == conversationId && userIds.Contains(x.MemberId))
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

            if (mention.Start < 0 || mention.Length <= 0 || mention.Start > content.Length - mention.Length)
            {
                continue;
            }

            var mentionedText = content.Substring(mention.Start, mention.Length);

            if (mentionedText != $"@{user.Name}")
                continue;

            // Domain sẽ kiểm tra range hợp lệ
            var validMention = Mention.Create(user.Id, mention.Start, mention.Length);
            validMentions.Add(validMention);
        }

        return validMentions;
    }
}

public class SendMessageRequest
{
    public string? Content { get; set; }
    public MessageType MessageType { get; set; }
    public Guid? ReferenceId { get; set; } // Dùng cho loại Sticker / Media
    public List<MentionDto> Mentions { get; set; } = [];
    public long? ReplyToMessageId { get; set; }
}
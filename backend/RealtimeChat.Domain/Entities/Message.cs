using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Message : BaseEntity
{
    public long ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;

    // Người gửi, null khi là tin nhắn hệ thống
    public long? SenderId { get; private set; }
    public User? Sender { get; private set; }

    public MessageType Type { get; private set; }
    public string? Content { get; private set; }

    // Các payload cho từng loại message
    public long? MediaId { get; private set; }
    public Media? Media { get; private set; }

    public long? CallId { get; private set; }
    public Call? Call { get; private set; }

    public long? StickerId { get; private set; }
    public Sticker? Sticker { get; private set; }
    //

    public long? ReplyToMessageId { get; private set; }
    public Message? ReplyToMessage { get; private set; }

    public DateTime? RecalledAt { get; private set; }

    private readonly List<Mention> _mentions = [];
    public IReadOnlyCollection<Mention> Mentions => _mentions.AsReadOnly();

    // Token mention dùng để xác định vị trí mention trong nội dung tin nhắn. "Ví dụ, hello <@1>"
    private static string MentionToken(long userId) => $"<@{userId}>";
    private static readonly TimeSpan RecallWindow = TimeSpan.FromMinutes(15);

    private Message() { }

    # region Create

    public static Message CreateText(long conversationId, long senderId, string content, ICollection<long>? mentionUserIds = null, Message? replyToMessage = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Message content cannot be empty.");

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Type = MessageType.Text,
            Content = content,
        };

        message.AddMentions(mentionUserIds);

        if (replyToMessage is not null)
            message.SetReplyTo(replyToMessage);

        return message;
    }

    public static Message CreateMedia(long conversationId, long senderId, Media media)
    {
        return new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Type = MessageType.Media,
            MediaId = media.Id,
            Media = media,
        };
    }

    public static Message CreateSticker(long conversationId, long senderId, Sticker sticker)
    {
        return new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Type = MessageType.Sticker,
            StickerId = sticker.Id,
            Sticker = sticker,
        };
    }

    public static Message CreateCall(long conversationId, Call call)
    {
        return new Message
        {
            ConversationId = conversationId,
            SenderId = call.CallerId,
            Type = MessageType.Call,
            CallId = call.Id,
            Call = call,
        };
    }

    public static Message CreateSystem(long conversationId, string content, ICollection<long>? mentionUserIds = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Content is required");    

        var message = new Message
        {
            ConversationId = conversationId,
            Type = MessageType.System,
            Content = content,
        };

        message.AddMentions(mentionUserIds);

        return message;
    }

    #endregion

    public bool Recall(long actorId, DateTime now)
    {
        if (actorId != SenderId)
            throw new ForbiddenDomainException();

        if (RecalledAt.HasValue)
            return false;

        if (Type is MessageType.System or MessageType.Call)
            throw new DomainException("This message type cannot be recalled.");

        if (now > CreatedAt.Add(RecallWindow))
            throw new DomainException("Message can no longer be recalled.");

        Content = null;
        _mentions.Clear();
        RecalledAt = now;
        return true;
    }

    private void SetReplyTo(Message message)
    {
        if (ConversationId != message.ConversationId)
            throw new DomainException("Cannot reply to a message in another conversation.");

        if (message.Type == MessageType.System)
            throw new DomainException("Cannot reply to a system message.");

        ReplyToMessageId = message.Id;
        ReplyToMessage = message;
    }

    private void AddMentions(ICollection<long>? mentionUserIds)
    {
        if (string.IsNullOrWhiteSpace(Content))
            throw new DomainException("Cannot add mentions to a message with no content.");

        foreach (var userId in mentionUserIds?.Distinct() ?? [])
        {
            var token = MentionToken(userId);

            if (Content.Contains(token, StringComparison.Ordinal))
                _mentions.Add(Mention.Create(userId));
        }
    }
}
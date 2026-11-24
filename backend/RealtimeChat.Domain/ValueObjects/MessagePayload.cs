using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.ValueObjects;

public abstract record MessagePayload(MessageType Type);

public record TextPayload(string Content, IReadOnlyList<long> MentionUserIds)
    : MessagePayload(MessageType.Text)
{
    public static TextPayload Create(string content, IEnumerable<long>? mentionUserIds = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Message content cannot be empty.");

        var mentions = mentionUserIds?.Distinct()
            .Where(id => content.Contains(ChatMessage.MentionToken(id), StringComparison.Ordinal))
            .ToList() ?? [];

        return new TextPayload(content, mentions);
    }
}

public record SystemPayload(SystemMessageType Action, long ActorId, long? TargetUserId = null)
    : MessagePayload(MessageType.System);

public record MediaPayload(Guid PublicId, string OriginalName, string ContentType, long Size, string StorageKey)
    : MessagePayload(MessageType.Media)
{
    public static MediaPayload From(Media media) =>
        new(media.PublicId, media.OriginalName, media.ContentType, media.Size, media.StorageKey);
}

public record CallPayload(long CallId, CallType CallType, CallStatus Status, TimeSpan? Duration)
    : MessagePayload(MessageType.Call)
{
    public static CallPayload From(Call call) =>
        new(call.Id, call.Type, call.Status, call.Duration);
}

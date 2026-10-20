using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Domain.Factories;

public static class SystemMessageFactory
{
    public static Message CreateMemberAdded(long conversationId, long actorId, long targetUserId)
    {
        var content = $"{Message.MentionToken(actorId)} đã thêm {Message.MentionToken(targetUserId)} vào nhóm";
        return Message.CreateSystem(conversationId, content, [actorId, targetUserId]);
    }

    public static Message CreateMemberJoined(long conversationId, long actorId)
    {
        var content = $"{Message.MentionToken(actorId)} đã tham gia nhóm qua liên kết mời";
        return Message.CreateSystem(conversationId, content, [actorId]);
    }

    public static Message CreateGroupDisbanded(long conversationId, long actorId)
    {
        var content = $"{Message.MentionToken(actorId)} đã giải tán nhóm";
        return Message.CreateSystem(conversationId, content, [actorId]);
    }

    public static Message CreateMemberKicked(long conversationId, long actorId, long targetUserId)
    {
        var content = $"{Message.MentionToken(actorId)} đã xóa {Message.MentionToken(targetUserId)} khỏi nhóm";
        return Message.CreateSystem(conversationId, content, [actorId, targetUserId]);
    }

    public static Message CreateMemberLeft(long conversationId, long actorId)
    {
        var content = $"{Message.MentionToken(actorId)} đã rời nhóm";
        return Message.CreateSystem(conversationId, content, [actorId]);
    }

    public static Message CreateAdminTransferred(long conversationId, long actorId, long targetUserId)
    {
        var content = $"{Message.MentionToken(actorId)} đã chuyển quyền trưởng nhóm cho {Message.MentionToken(targetUserId)}";
        return Message.CreateSystem(conversationId, content, [actorId, targetUserId]);
    }
}

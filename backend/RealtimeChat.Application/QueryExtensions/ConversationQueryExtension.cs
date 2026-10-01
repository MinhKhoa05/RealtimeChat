using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.QueryExtensions;

public static class ConversationQueryExtensions
{
    public static IQueryable<Conversation> GetById(this IQueryable<Conversation> query, long conversationId)
        => query.Where(x => x.Id == conversationId);

    public static IQueryable<Conversation> Active(this IQueryable<Conversation> query)
        => query.Where(x => x.DisbandedAt == null);

    public static IQueryable<Conversation> Groups(this IQueryable<Conversation> query)
        => query.Where(x => x.Type == ConversationType.Group);

    public static IQueryable<Conversation> Group(this IQueryable<Conversation> query, long groupId)
        => query.GetById(groupId).Groups();

    public static IQueryable<Conversation> Directs(this IQueryable<Conversation> query)
        => query.Where(x => x.Type == ConversationType.Direct);

    public static IQueryable<Conversation> Direct(this IQueryable<Conversation> query, long user1Id, long user2Id)
    {
        var min = Math.Min(user1Id, user2Id);
        var max = Math.Max(user1Id, user2Id);

        var directKey = $"{min}:{max}";

        return query.Directs().Where(x => x.DirectKey == directKey);
    }

    public static IQueryable<Conversation> AccessibleBy(this IQueryable<Conversation> query, long userId)
        => query.Where(x => x.Members.Any(m => m.MemberId == userId));
}
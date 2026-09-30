using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.QueryExtensions;

public static class ConversationQueryExtensions
{
    public static IQueryable<Conversation> GetById(this IQueryable<Conversation> query, long conversationId)
        => query.Where(x => x.Id == conversationId);

    public static IQueryable<Conversation> Groups(this IQueryable<Conversation> query)
        => query.Where(x => x.Type == ConversationType.Group);

    public static IQueryable<Conversation> Group(this IQueryable<Conversation> query, long groupId)
        => query.GetById(groupId).Groups();

    public static IQueryable<Conversation> Directs(this IQueryable<Conversation> query)
        => query.Where(x => x.Type == ConversationType.Direct);
}
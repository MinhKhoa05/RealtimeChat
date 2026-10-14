using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace RealtimeChat.Application.QueryExtensions;

public static class ConversationQueryExtensions
{
    public static IQueryable<Conversation> WithDirectKey(this IQueryable<Conversation> query, string directKey)
        => query.Where(x => x.Type == ConversationType.Direct && x.DirectKey == directKey);

    public static IQueryable<Conversation> Active(this IQueryable<Conversation> query)
        => query.Where(x => x.DisbandedAt == null);

    public static IQueryable<Conversation> FilterAccessible(this IQueryable<Conversation> query, long conversationId, long userId)
        => query.Active().AccessibleBy(userId).Where(x => x.Id == conversationId);

    public static IQueryable<Conversation> AccessibleBy(this IQueryable<Conversation> query, long userId)
        => query.Where(x => x.Members.Any(m => m.MemberId == userId));

    public static IQueryable<Conversation> FilterAccessibleGroup(this IQueryable<Conversation> query, long groupId, long userId)
        => query.FilterAccessible(groupId, userId).Where(x => x.Type == ConversationType.Group);

    // Chỉ dùng khi cần dữ liệu thành viên để tránh tải dữ liệu không cần thiết.
    public static IQueryable<Conversation> WithMembers(this IQueryable<Conversation> query, params long[] userIds)
        => query.Include(x => x.Members.Where(m => userIds.Contains(m.MemberId)));
}
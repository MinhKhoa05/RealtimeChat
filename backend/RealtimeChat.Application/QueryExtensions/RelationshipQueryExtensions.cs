using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.QueryExtensions;

public static class RelationshipQueryExtensions
{
    public static IQueryable<Relationship> Friends(this IQueryable<Relationship> query)
        => query.Where(x => x.Type == RelationshipType.Friend);

    public static IQueryable<Relationship> Friend(this IQueryable<Relationship> query, long userId, long friendId)
    {
        var user1Id = Math.Min(userId, friendId);
        var user2Id = Math.Max(userId, friendId);

        return query.Friends().Where(x => x.UserId == user1Id && x.TargetUserId == user2Id);
    }

    public static IQueryable<Relationship> FriendRequests(this IQueryable<Relationship> query)
        => query.Where(x => x.Type == RelationshipType.FriendRequest);

    public static IQueryable<Relationship> Blocks(this IQueryable<Relationship> query)
        => query.Where(x => x.Type == RelationshipType.Block);

    public static IQueryable<Relationship> Block(this IQueryable<Relationship> query, long blockerId, long blockedId)
        => query.Blocks().Where(x => x.UserId == blockerId && x.TargetUserId == blockedId);

    public static IQueryable<Relationship> Between(this IQueryable<Relationship> query, long userId, long otherUserId)
        => query.Where(x =>
            (x.UserId == userId && x.TargetUserId == otherUserId) ||
            (x.UserId == otherUserId && x.TargetUserId == userId));

    public static IQueryable<Relationship> WithUsers(this IQueryable<Relationship> query, long userId, IEnumerable<long> otherUserIds)
        => query.Where(x =>
            (x.UserId == userId && otherUserIds.Contains(x.TargetUserId)) ||
            (otherUserIds.Contains(x.UserId) && x.TargetUserId == userId));
}
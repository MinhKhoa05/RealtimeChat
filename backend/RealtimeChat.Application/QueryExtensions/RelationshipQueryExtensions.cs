using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.QueryExtensions;

public static class RelationshipQueryExtensions
{
    public static IQueryable<Relationship> FriendRequests(this IQueryable<Relationship> query)
        => query.Where(x => x.Type == RelationshipType.FriendRequest);

    public static IQueryable<Relationship> FriendRequest(this IQueryable<Relationship> query, long requestId)
        => query.FriendRequests().Where(x => x.Id == requestId);

    public static IQueryable<Relationship> Blocks(this IQueryable<Relationship> query)
        => query.Where(x => x.Type == RelationshipType.Block);

    public static IQueryable<Relationship> Friends(this IQueryable<Relationship> query)
        => query.Where(x => x.Type == RelationshipType.Friend);
    
    public static IQueryable<Relationship> OfUsers(this IQueryable<Relationship> query, long userId)
        => query.Where(x => x.UserId == userId || x.TargetUserId == userId);

    public static IQueryable<Relationship> Between(this IQueryable<Relationship> query, long userId, long otherUserId)
        => query.Where(x =>
            (x.UserId == userId && x.TargetUserId == otherUserId) ||
            (x.UserId == otherUserId && x.TargetUserId == userId));

    public static IQueryable<Relationship> WithUsers(this IQueryable<Relationship> query, long userId, IEnumerable<long> otherUserIds)
        => query.Where(x =>
            (x.UserId == userId && otherUserIds.Contains(x.TargetUserId)) ||
            (otherUserIds.Contains(x.UserId) && x.TargetUserId == userId));
}   
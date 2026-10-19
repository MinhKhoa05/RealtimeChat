using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace RealtimeChat.Application.QueryExtensions;

public static class ConversationQueryExtensions
{
    public static Task<Conversation?> GetWithMembersAsync(this IQueryable<Conversation> query, long conversationId, IEnumerable<long> includeMemberIds, CancellationToken ct)
    {
        return query
            .Where(x => x.Id == conversationId && !x.DisbandedAt.HasValue)
            .Include(x => x.Members.Where(m => includeMemberIds.Contains(m.MemberId)))
            .FirstOrDefaultAsync(ct);
    }

    public static Task<Conversation?> GetWithMembersByInviteCodeAsync(this IQueryable<Conversation> query, Guid inviteCode, IEnumerable<long> includeMemberIds, CancellationToken ct)
    {
        return query
            .Where(x => x.InviteCode == inviteCode && !x.DisbandedAt.HasValue)
            .Include(x => x.Members.Where(m => includeMemberIds.Contains(m.MemberId)))
            .FirstOrDefaultAsync(ct);
    }

    public static IQueryable<ConversationMember> GetMembers(this IQueryable<Conversation> query, IReadOnlyCollection<long> conversationIds)
        => query
            .Where(x => conversationIds.Contains(x.Id) && !x.DisbandedAt.HasValue)
            .SelectMany(x => x.Members);

    public static IQueryable<ConversationMember> GetMembers(this IQueryable<Conversation> query, long conversationId)
        => query.GetMembers([conversationId]);

    public static Task<bool> IsMemberAsync(this IQueryable<Conversation> query, long conversationId, long userId, CancellationToken ct)
        => query.GetMembers(conversationId).Where(x => x.MemberId == userId).AnyAsync(ct); 

    public static IQueryable<Conversation> WithDirectKey(this IQueryable<Conversation> query, string directKey)
        => query.Where(x => x.Type == ConversationType.Direct && x.DirectKey == directKey);
}
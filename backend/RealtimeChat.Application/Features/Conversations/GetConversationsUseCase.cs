using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Conversations;

public class GetConversationsUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetConversationsUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ConversationItem>> ExecuteAsync(ConversationType? type, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var query = _context.Conversations
            .AsNoTracking()
            .AccessibleBy(currentUserId)
            .Active();

        if (type.HasValue)
        {
            query = query.Where(x => x.Type == type.Value);
        }

        return await query
            .Select(x => new ConversationItem
            {
                Id = x.Id,
                Type = x.Type,
                AvatarUrl = null,

                Name = x.Type == ConversationType.Direct
                    ? x.Members
                        .Where(m => m.MemberId != currentUserId)
                        .Select(m => m.Member.Name)
                        .FirstOrDefault()
                    : x.Name,

                IsPinned = x.Members
                    .Where(m => m.MemberId == currentUserId)
                    .Select(m => m.IsPinned)
                    .FirstOrDefault(),
            })
            .OrderByDescending(x => x.IsPinned)
            .ToListAsync(ct);
    }
}

public class ConversationItem
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public ConversationType Type { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsPinned { get; set; }
}
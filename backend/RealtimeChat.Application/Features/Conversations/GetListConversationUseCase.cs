using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Conversations;

public class GetListConversationUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetListConversationUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ConversationItem>> ExecuteAsync(CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var conversations = await _context.Conversations
            .AsNoTracking()
            .Active()
            .AccessibleBy(currentUserId)
            .Select(x => new ConversationItem
            {
                Id = x.Id,
                Type = x.Type,
                AvatarUrl = null,

                Name = x.Type == ConversationType.Direct
                    ? x.Members.First(m => m.MemberId != currentUserId).Member.Name
                    : x.Name,

                IsPinned = x.Members.First(m => m.MemberId != currentUserId).IsPinned,
            })
            .OrderByDescending(x => x.IsPinned)
            .ToListAsync(ct);

        return conversations;
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
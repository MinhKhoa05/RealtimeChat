using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Users;

public class SearchUsersUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public SearchUsersUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<SearchUsersResponse>> ExecuteAsync(
    string keyword,
    CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var usersQuery = _context.Users
            .AsNoTracking()
            .Where(x =>
                x.Id != currentUserId &&
                (x.Email.Contains(keyword) ||
                 x.Name.Contains(keyword)));

        var resultsQuery = usersQuery
            .Select(user => new SearchUsersResponse
            {
                UserId = user.Id,
                Name = user.Name,
                AvatarUrl = null,
                Status = _context.Relationships
                    .Between(currentUserId, user.Id)
                    .Select(x => (RelationshipStatus?)x.Type)
                    .FirstOrDefault() ?? RelationshipStatus.None
            });

        return await resultsQuery.ToListAsync(ct);
    }
}

public class SearchUsersResponse
{
    public long UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public RelationshipStatus Status { get; set; }
}

public enum RelationshipStatus
{
    None,
    Friend,
    SentRequest,
    ReceivedRequest,
    Blocked,
    BlockedByUser
}

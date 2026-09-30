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

    public async Task<List<SearchUsersResponse>> ExecuteAsync(string keyword, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var users = await _context.Users
            .AsNoTracking()
            .Where(x => x.Id != currentUserId && (x.Email.Contains(keyword) || x.Name.Contains(keyword)))
            .ToListAsync(ct);

        var userIds = users.Select(x => x.Id);

        var relationships = await _context.Relationships
            .AsNoTracking()
            .WithUsers(currentUserId, userIds)
            .ToListAsync(ct);

        var relationshipMaps = relationships.ToDictionary(
            x => x.UserId == currentUserId
                ? x.TargetUserId
                : x.UserId);

        var results = users.Select(user =>
        {
            relationshipMaps.TryGetValue(user.Id, out var relationship);

            return new SearchUsersResponse
            {
                UserId = user.Id,
                Name = user.Name,
                AvatarUrl = null,
                Status = MapToRelationshipStatus(relationship, currentUserId)
            };
        }).ToList();

        return results;
    }

    private RelationshipStatus MapToRelationshipStatus(Relationship? relationship, long currentUserId)
    {
        return relationship?.Type switch
        {
            RelationshipType.Friend => RelationshipStatus.Friend,

            RelationshipType.FriendRequest =>
                relationship.UserId == currentUserId
                    ? RelationshipStatus.SentRequest
                    : RelationshipStatus.ReceivedRequest,

            RelationshipType.Block =>
                relationship.UserId == currentUserId
                    ? RelationshipStatus.Blocked
                    : RelationshipStatus.BlockedByUser,

            _ => RelationshipStatus.None
        };
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

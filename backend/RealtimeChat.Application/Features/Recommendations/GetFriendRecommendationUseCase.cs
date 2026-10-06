using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Features.Users;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.Features.Recommendations.Services;

namespace RealtimeChat.Application.Features.Recommendations;

public class GetFriendRecommendationUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IFriendRecommendationService _friendRecommendation;

    public GetFriendRecommendationUseCase(IAppDbContext context, ICurrentUser currentUser, IFriendRecommendationService friendRecommendation)
    {
        _context = context;
        _currentUser = currentUser;
        _friendRecommendation = friendRecommendation;
    }

    public async Task<List<UserResposne>> ExecuteAsync(int limit, CancellationToken ct)
    {
        if (limit < 1 || limit > 15)
        {
            throw new BadRequestException("Limit must be from 1 - 15");
        }

        var userIds = await _friendRecommendation.GetRecommendationsAsync(_currentUser.UserId, limit, ct);

        if (userIds.Count == 0) return [];

        var users = await _context.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new UserResposne
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                AvatarUrl = null
            })
            .ToListAsync(ct);

        return users;
    }
}
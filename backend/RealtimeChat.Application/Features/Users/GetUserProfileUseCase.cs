using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Users;

public class GetUserProfileUseCase
{
    private readonly IAppDbContext _context;

    public GetUserProfileUseCase(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<UserResposne> ExecuteAsync(long userId, CancellationToken ct)
    {
        var user = await _context.Users.FindAsync(userId, ct)
            ?? throw new Exception("User not found");

        return new UserResposne
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            AvatarUrl = null
        };
    }
}
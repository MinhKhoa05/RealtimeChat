using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.Exceptions;

namespace RealtimeChat.Application.Features.Users;

public class GetUserDetailUseCase
{
    private readonly IAppDbContext _context;

    public GetUserDetailUseCase(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDetailResponse> ExecuteAsync(long userId, CancellationToken ct)
    {
        var user = await _context.Users.FindAsync(userId, ct)
            ?? throw new NotFoundException();

        return new UserDetailResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            AvatarUrl = null
        };
    }
}
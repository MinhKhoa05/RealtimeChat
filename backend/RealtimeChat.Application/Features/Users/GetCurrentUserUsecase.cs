using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Users;

public class GetCurrentUserUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetCurrentUserUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<UserResposne> ExecuteAsync(CancellationToken ct)
    {
        var user = await _context.Users.FindAsync(_currentUser.UserId, ct)
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

public class UserResposne
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }
}
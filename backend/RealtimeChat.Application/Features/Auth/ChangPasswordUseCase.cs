using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Auth;

public class ChangePasswordUseCase
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUser _currentUser;

    public ChangePasswordUseCase(IAppDbContext context, ICurrentUser currentUser, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(ChangePasswordRequest request, CancellationToken ct)
    {
        var user = await _context.Users.FindAsync(_currentUser.UserId)
            ?? throw new Exception("User not found");

        var isMatch = _passwordHasher.Verify(request.CurrentPassword, user.Password);
        if (!isMatch)
        {
            throw new Exception("Invalid Current Password");
        }

        user.Password = _passwordHasher.Hash(request.NewPassword);

        await _context.SaveChangesAsync(ct);
    }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
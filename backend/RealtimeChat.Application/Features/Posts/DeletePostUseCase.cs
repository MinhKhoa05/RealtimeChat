using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Posts;

public class DeletePostUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;

    public DeletePostUseCase(IAppDbContext context, ICurrentUser currentUser, TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(long postId, CancellationToken ct)
    {
        var post = await _context.Posts.FindAsync(postId, ct)
            ?? throw new NotFoundException();

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        post.Delete(_currentUser.UserId, now);

        await _context.SaveChangesAsync(ct);
    }
}
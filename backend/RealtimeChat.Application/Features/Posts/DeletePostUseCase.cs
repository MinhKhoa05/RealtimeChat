using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Posts;

public class DeletePostUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public DeletePostUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long postId, CancellationToken ct)
    {
        var post = await _context.Posts.FindAsync(postId, ct)
            ?? throw new NotFoundException();

        post.Delete(_currentUser.UserId, DateTime.UtcNow);

        await _context.SaveChangesAsync(ct);
    }
}
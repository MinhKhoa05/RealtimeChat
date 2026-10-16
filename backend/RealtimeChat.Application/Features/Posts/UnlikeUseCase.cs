using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Posts;

public class UnlikePostUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UnlikePostUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long postId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var like = await _context.PostLikes.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == currentUserId, ct);

        if (like is null)
            return;

        _context.PostLikes.Remove(like);
        await _context.SaveChangesAsync(ct);
    }
}
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Posts;

public class LikePostUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public LikePostUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long postId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId && x.DeletedAt == null, ct)
            ?? throw new NotFoundException();

        var alreadyLike = await _context.PostLikes
            .AnyAsync(x => x.PostId == postId && x.UserId == currentUserId, ct);

        if (alreadyLike)
            return;

        var like = PostLike.Create(postId, currentUserId);
        _context.PostLikes.Add(like);

        await _context.SaveChangesAsync(ct);
    }
}
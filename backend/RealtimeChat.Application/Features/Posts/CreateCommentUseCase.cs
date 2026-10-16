using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Posts;

public class CreateCommentUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CreateCommentUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<long> ExecuteAsync(long postId, CreateCommentRequest request, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId && x.DeletedAt == null, ct)
            ?? throw new NotFoundException();

        var comment = Comment.Create(postId, actorId, request.Content);
        _context.Comments.Add(comment);

        await _context.SaveChangesAsync(ct);

        return comment.Id;
    }
}

public class CreateCommentRequest
{
    public string Content { get; set; } = null!;
}
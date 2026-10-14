using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Posts;

public class CreatePostUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CreatePostUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<long> ExecuteAsync(CreatePostRequest request, CancellationToken ct)
    {
        var post = Post.Create(_currentUser.UserId, request.Content, request.Privacy);

        _context.Posts.Add(post);

        await _context.SaveChangesAsync(ct);
        return post.Id;
    }
}

public class CreatePostRequest
{
    public string Content { get; set; } = null!;
    public PostPrivacy Privacy { get; set; }
}
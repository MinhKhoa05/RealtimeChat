using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Posts;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Route("api/posts")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly CreatePostUseCase _createPost;
    private readonly DeletePostUseCase _deletePost;
    private readonly LikePostUseCase _likePost;
    private readonly UnlikePostUseCase _unlikePost;
    private readonly CreateCommentUseCase _createComment;

    public PostsController(
        CreatePostUseCase createPost,
        DeletePostUseCase deletePost,
        LikePostUseCase likePost,
        UnlikePostUseCase unlikePost,
        CreateCommentUseCase createComment)
    {
        _createPost = createPost;
        _deletePost = deletePost;
        _likePost = likePost;
        _unlikePost = unlikePost;
        _createComment = createComment;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request, CancellationToken ct)
    {
        var postId = await _createPost.ExecuteAsync(request, ct);
        return Ok(postId);
    }

    [HttpDelete("{postId:long}")]
    public async Task<IActionResult> DeletePost(long postId, CancellationToken ct)
    {
        await _deletePost.ExecuteAsync(postId, ct);
        return NoContent();
    }

    [HttpPost("{postId:long}/likes")]
    public async Task<IActionResult> LikePost(long postId, CancellationToken ct)
    {
        await _likePost.ExecuteAsync(postId, ct);
        return NoContent();
    }

    [HttpDelete("{postId:long}/likes")]
    public async Task<IActionResult> UnlikePost(long postId, CancellationToken ct)
    {
        await _unlikePost.ExecuteAsync(postId, ct);
        return NoContent();
    }

    [HttpPost("{postId:long}/comments")]
    public async Task<IActionResult> CreateComment(long postId, [FromBody] CreateCommentRequest request, CancellationToken ct)
    {
        var commentId = await _createComment.ExecuteAsync(postId, request, ct);
        return Ok(commentId);
    }
}
namespace RealtimeChat.Application.Features.Messages;

public class MentionDto
{
    public long UserId { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
}
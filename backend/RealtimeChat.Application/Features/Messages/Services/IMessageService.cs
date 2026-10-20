using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Messages.Services;

public interface IMessageService
{
    Task ValidateCanSendMessageAsync(long conversationId, long senderId, CancellationToken ct);
    Task NotifyMessageCreatedAsync(Message message, CancellationToken ct);
}
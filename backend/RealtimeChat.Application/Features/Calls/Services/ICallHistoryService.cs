using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Calls.Services;

public interface ICallHistoryService
{
    Task<Message> CreateAsync(Call call, CancellationToken ct);
}
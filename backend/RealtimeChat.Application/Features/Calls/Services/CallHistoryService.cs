using RealtimeChat.Application.Features.Conversations.Services;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Calls.Services;

public class CallHistoryService : ICallHistoryService
{
    private readonly IAppDbContext _context;
    private readonly IConversationService _conversationService;

    public CallHistoryService(IAppDbContext context, IConversationService conversationService)
    {
        _context = context;
        _conversationService = conversationService;
    }

    public async Task<Message> CreateAsync(Call call, CancellationToken ct)
    {
        var conversation = await _conversationService.GetOrCreateDirectConversationAsync(call.CallerId, call.ReceiverId, ct);

        var message = Message.CreateCall(conversation.Id, call);
        _context.Messages.Add(message);

        return message;
    }
}
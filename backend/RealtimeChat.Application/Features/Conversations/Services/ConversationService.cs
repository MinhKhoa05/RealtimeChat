using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Conversations.Services;

public class ConversationService : IConversationService
{
    private readonly IAppDbContext _context;

    public ConversationService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Conversation> GetOrCreateDirectConversationAsync(long userId, long otherUserId, CancellationToken ct)
    {
        if (userId == otherUserId)
        {
            throw new BadRequestException("You cannot start a conversation with yourself.");
        }

        var conversation = await _context.Conversations.GetDirectConversationAsync(userId, otherUserId, ct);
        if (conversation is not null) return conversation;

        var existingUserIds = await _context.Users
            .Where(x => x.Id == userId || x.Id == otherUserId)
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (existingUserIds.Count != 2)
            throw new NotFoundException("User not found");

        conversation = Conversation.CreateDirect(userId, otherUserId);
        _context.Conversations.Add(conversation);

        try
        {
            await _context.SaveChangesAsync(ct);
            return conversation;
        }
        catch (DuplicateKeyException)
        {
            return (await _context.Conversations.GetDirectConversationAsync(userId, otherUserId, ct))!;
        }
    }
}
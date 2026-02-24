using ChatService.Application.DTOs;
using ChatService.Domain.Interfaces;
namespace ChatService.Application.Queries.GetConversations;
public sealed class GetConversationsHandler
{
    private readonly IMessageRepository _messages;
    public GetConversationsHandler(IMessageRepository messages) => _messages = messages;
    public async Task<IEnumerable<ConversationDto>> HandleAsync(GetConversationsQuery q, CancellationToken ct = default)
    {
        var summaries = await _messages.GetConversationSummariesAsync(q.UserId, ct);
        return summaries.Select(s => new ConversationDto(s.OtherUserId, s.OtherUserFirstName, s.OtherUserLastName, s.OtherUserAvatarUrl, s.LastMessageContent, s.LastMessageSentAt, s.LastMessageSenderId, s.UnreadCount));
    }
}

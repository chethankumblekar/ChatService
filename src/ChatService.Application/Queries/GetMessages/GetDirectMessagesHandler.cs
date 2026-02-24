using ChatService.Application.DTOs;
using ChatService.Application.Mappings;
using ChatService.Domain.Interfaces;
namespace ChatService.Application.Queries.GetMessages;
public sealed class GetDirectMessagesHandler
{
    private readonly IMessageRepository _messages;
    public GetDirectMessagesHandler(IMessageRepository messages) => _messages = messages;
    public async Task<IEnumerable<MessageDto>> HandleAsync(GetDirectMessagesQuery q, CancellationToken ct = default)
    {
        var take = Math.Clamp(q.Take, 1, 100);
        var msgs = await _messages.GetDirectMessagesAsync(q.UserId, q.RecipientId, q.Skip, take, ct);
        return msgs.Select(m => m.ToDto());
    }
}

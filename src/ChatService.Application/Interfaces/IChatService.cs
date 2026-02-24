using ChatService.Application.DTOs;
namespace ChatService.Application.Interfaces;
public interface IChatService
{
    Task<MessageDto> SendDirectMessageAsync(string senderId, string recipientId, string content, CancellationToken ct = default);
    Task<MessageDto> SendGroupMessageAsync(string senderId, Guid groupId, string content, CancellationToken ct = default);
    Task MarkMessageReadAsync(Guid messageId, string readerId, CancellationToken ct = default);
}

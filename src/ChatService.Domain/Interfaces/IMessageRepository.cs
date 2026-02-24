using ChatService.Domain.Entities;
namespace ChatService.Domain.Interfaces;
public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Message> AddAsync(Message message, CancellationToken ct = default);
    Task UpdateAsync(Message message, CancellationToken ct = default);
    Task<IEnumerable<Message>> GetDirectMessagesAsync(string u1, string u2, int skip, int take, CancellationToken ct = default);
    Task<IEnumerable<ConversationSummary>> GetConversationSummariesAsync(string userId, CancellationToken ct = default);
    Task MarkAsReadAsync(Guid messageId, CancellationToken ct = default);
    Task MarkAllReadAsync(string senderId, string recipientId, CancellationToken ct = default);
}
public record ConversationSummary(string OtherUserId, string OtherUserFirstName, string OtherUserLastName, string? OtherUserAvatarUrl, string LastMessageContent, DateTime LastMessageSentAt, string LastMessageSenderId, int UnreadCount);

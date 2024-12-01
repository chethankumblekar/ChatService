using PlayGround.ChatService.Model;

namespace PlayGround.ChatService.Core
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task<Message> GetMessageByIdAsync(string messageId);
        Task UpdateMessageAsync(Message message);
        Task DeleteMessageAsync(string messageId);
        Task<IEnumerable<Message>> GetMessagesForUserAsync(Guid userId);
        Task<IEnumerable<Message>> GetMessagesForGroupAsync(Guid groupId);
    }
}

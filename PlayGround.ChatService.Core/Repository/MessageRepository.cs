using Microsoft.EntityFrameworkCore;
using PlayGround.ChatService.Core;
using PlayGround.ChatService.Model;

namespace PlayGround.ChatService.Infrastructure.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        public MessageRepository(DataContext dataContext)
        {
            _context = dataContext;
        }
        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMessageAsync(string messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Message> GetMessageByIdAsync(string messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                return message;
            }
            throw new KeyNotFoundException();
        }

        public async Task<IEnumerable<Message>> GetMessagesForGroupAsync(Guid groupId)
        {
            return await _context.Messages
                            .Where(m => m.GroupId == groupId)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesForUserAsync(Guid userId)
        {
            return await _context.Messages
                            .Where(m => m.GroupId == userId)
                            .ToListAsync();
        }

        public async Task UpdateMessageAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }
    }
}

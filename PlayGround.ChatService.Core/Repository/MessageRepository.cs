using Microsoft.EntityFrameworkCore;
using PlayGround.ChatService.Core;
using PlayGround.ChatService.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayGround.ChatService.Infrastructure.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IDbContextFactory<DataContext> _dataContextFactory;

        public MessageRepository(IDbContextFactory<DataContext> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task AddMessageAsync(Message message)
        {
            using (var _context = _dataContextFactory.CreateDbContext())
            {
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteMessageAsync(string messageId)
        {
            using (var _context = _dataContextFactory.CreateDbContext())
            {
                var message = await _context.Messages.FindAsync(messageId);
                if (message != null)
                {
                    _context.Messages.Remove(message);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<Message> GetMessageByIdAsync(string messageId)
        {
            using (var _context = _dataContextFactory.CreateDbContext())
            {
                var message = await _context.Messages.FindAsync(messageId);
                if (message != null)
                {
                    return message;
                }
                throw new KeyNotFoundException();
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesForGroupAsync(Guid groupId)
        {
            using (var _context = _dataContextFactory.CreateDbContext())
            {
                return await _context.Messages
                                .Where(m => m.GroupId == groupId)
                                .ToListAsync();
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesForUserAsync(string userId)
        {
            using (var _context = _dataContextFactory.CreateDbContext())
            {
                return await _context.Messages
                                .Where(m => m.SenderId == userId)
                                .ToListAsync();
            }
        }

        public async Task<List<Message?>> GetLastMessagesToOrFromUserAsync(string userId)
        {
            using (var _context = _dataContextFactory.CreateDbContext())
            {
                return await _context.Messages
                        .Where(m => m.SenderId == userId || m.RecipientId == userId)
                        .GroupBy(m => new
                        {
                            User1 = string.Compare(m.SenderId, m.RecipientId) < 0 ? m.SenderId : m.RecipientId,
                            User2 = string.Compare(m.SenderId, m.RecipientId) < 0 ? m.RecipientId : m.SenderId
                        })
                        .Select(g => g.OrderByDescending(m => m.SentAt).FirstOrDefault())
                        .ToListAsync();
            }
        }


        public async Task<List<Message>> GetOneOnOneMessagesAsync(string currentuserId, string recipientId)
        {
            using (var _context = _dataContextFactory.CreateDbContext())
            {
                return await _context.Messages
                        .Where(m => (m.SenderId == currentuserId && m.RecipientId == recipientId) 
                        || (m.SenderId == recipientId && m.RecipientId == currentuserId))
                        .OrderBy(m => m.SentAt)
                        .ToListAsync();
            }
        }


        public async Task UpdateMessageAsync(Message message)
        {
            using (var _context = _dataContextFactory.CreateDbContext())
            {
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}

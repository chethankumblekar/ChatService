using ChatService.Domain.Entities;
using ChatService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure.Persistence.Repositories;

public sealed class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _db;
    public MessageRepository(AppDbContext db) => _db = db;

    public Task<Message?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Messages.Include(m => m.Sender).FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<Message> AddAsync(Message message, CancellationToken ct = default)
    {
        _db.Messages.Add(message);
        await _db.SaveChangesAsync(ct);
        return message;
    }

    public async Task UpdateAsync(Message message, CancellationToken ct = default)
    {
        _db.Messages.Update(message);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Message>> GetDirectMessagesAsync(string u1, string u2, int skip, int take, CancellationToken ct = default) =>
        await _db.Messages
            .Where(m => (m.SenderId == u1 && m.RecipientId == u2) || (m.SenderId == u2 && m.RecipientId == u1))
            .OrderBy(m => m.SentAt).Skip(skip).Take(take).ToListAsync(ct);

    public async Task<IEnumerable<ConversationSummary>> GetConversationSummariesAsync(string userId, CancellationToken ct = default)
    {
        var messages = await _db.Messages
            .Where(m => (m.SenderId == userId || m.RecipientId == userId) && m.GroupId == null)
            .Include(m => m.Sender).Include(m => m.Recipient)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync(ct);

        return messages
            .GroupBy(m => m.SenderId == userId ? m.RecipientId! : m.SenderId)
            .Select(g =>
            {
                var last  = g.First();
                var other = last.SenderId == userId ? last.Recipient! : last.Sender;
                var unread = g.Count(m => m.SenderId != userId && m.ReadAt == null);
                return new ConversationSummary(g.Key, other?.FirstName ?? "", other?.LastName ?? "",
                    other?.AvatarUrl, last.Content, last.SentAt, last.SenderId, unread);
            })
            .OrderByDescending(s => s.LastMessageSentAt).ToList();
    }

    public async Task MarkAsReadAsync(Guid messageId, CancellationToken ct = default) =>
        await _db.Messages.Where(m => m.Id == messageId)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.ReadAt, DateTime.UtcNow), ct);

    public async Task MarkAllReadAsync(string senderId, string recipientId, CancellationToken ct = default) =>
        await _db.Messages.Where(m => m.SenderId == senderId && m.RecipientId == recipientId && m.ReadAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.ReadAt, DateTime.UtcNow), ct);
}

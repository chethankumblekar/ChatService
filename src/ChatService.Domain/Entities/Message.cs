using ChatService.Domain.Enums;

namespace ChatService.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string SenderId { get; private set; } = default!;
    public string? RecipientId { get; private set; }
    public Guid? GroupId { get; private set; }
    public string Content { get; private set; } = default!;
    public DateTime SentAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public MessageType Type { get; private set; }

    public User Sender { get; private set; } = default!;
    public User? Recipient { get; private set; }
    public Group? Group { get; private set; }

    protected Message() { }

    public static Message CreateDirect(string senderId, string recipientId, string content)
    {
        if (content.Length > 4000) throw new ArgumentException("Message cannot exceed 4000 chars.");
        return new() { SenderId = senderId, RecipientId = recipientId, Content = content.Trim(), SentAt = DateTime.UtcNow, Type = MessageType.Direct };
    }

    public static Message CreateGroup(string senderId, Guid groupId, string content) =>
        new() { SenderId = senderId, GroupId = groupId, Content = content.Trim(), SentAt = DateTime.UtcNow, Type = MessageType.Group };

    public void MarkAsRead() { if (!ReadAt.HasValue) { ReadAt = DateTime.UtcNow; UpdatedAt = DateTime.UtcNow; } }

    public void SoftDelete(string requestingUserId)
    {
        if (SenderId != requestingUserId) throw new UnauthorizedAccessException("Only the sender can delete a message.");
        IsDeleted = true; UpdatedAt = DateTime.UtcNow;
    }
}

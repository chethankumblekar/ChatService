namespace ChatService.Domain.Entities;

public class User
{
    public string Email { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string? AvatarUrl { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastSeenAt { get; private set; } = DateTime.UtcNow;

    private readonly List<Message> _sentMessages = [];
    private readonly List<Message> _receivedMessages = [];

    public ICollection<Message> SentMessages => _sentMessages;
    public ICollection<Message> ReceivedMessages => _receivedMessages;

    protected User() { }

    public static User Create(string email, string firstName, string lastName, string? avatarUrl = null) =>
        new() { Email = email.ToLowerInvariant(), FirstName = firstName.Trim(), LastName = lastName.Trim(), AvatarUrl = avatarUrl };

    public void UpdateProfile(string firstName, string lastName, string? avatarUrl)
    {
        FirstName = firstName.Trim(); LastName = lastName.Trim(); AvatarUrl = avatarUrl; UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastSeen() => LastSeenAt = DateTime.UtcNow;
    public string FullName => $"{FirstName} {LastName}".Trim();
}

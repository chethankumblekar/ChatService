namespace ChatService.Domain.Entities;

public class Group
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = string.Empty;
    public string CreatedById { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    private readonly List<User> _members = [];
    private readonly List<Message> _messages = [];
    public ICollection<User> Members => _members;
    public ICollection<Message> Messages => _messages;

    protected Group() { }

    public static Group Create(string name, string createdById, string description = "") =>
        new() { Name = name.Trim(), CreatedById = createdById, Description = description.Trim() };

    public void AddMember(User user) { if (!_members.Any(m => m.Email == user.Email)) { _members.Add(user); UpdatedAt = DateTime.UtcNow; } }
    public void RemoveMember(string userEmail) { var m = _members.FirstOrDefault(u => u.Email == userEmail); if (m is not null) { _members.Remove(m); UpdatedAt = DateTime.UtcNow; } }
}

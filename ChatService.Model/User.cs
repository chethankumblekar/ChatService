using System.ComponentModel.DataAnnotations;

namespace PlayGround.ChatService.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public List<Message> Messages { get; set; } = new();
        public List<Group> Groups { get; set; } = new();
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; } = string.Empty;
    }
}

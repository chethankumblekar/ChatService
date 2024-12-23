using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayGround.ChatService.Model
{
    public class Group
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public List<User> Members { get; set; } = new();

        public List<Message> Messages { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayGround.ChatService.Model
{
    [Index(nameof(SenderId))]
    [Index(nameof(GroupId))]
    public class Message
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }

        public virtual User Sender { get; set; } // Navigation property

        public Guid? RecipientId { get; set; }

        public virtual User? Recipient { get; set; } // Nullable for group messages

        public Guid? GroupId { get; set; }

        public virtual Group? Group { get; set; } // Nullable for user-to-user messages

        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

}

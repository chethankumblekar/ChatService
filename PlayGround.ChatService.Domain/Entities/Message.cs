using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayGround.ChatService.Domain.Entities
{
    public class Message
    {
        public required string Id { get; set; }

        public required string SenderId { get; set; }

        public DateTime SentTime  { get; set; }

        public required string content { get; set; }
    }
}

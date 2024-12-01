﻿using System.ComponentModel.DataAnnotations;

namespace PlayGround.ChatService.Domain.Entities
{
    public class User
    {
        public required string UserId { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

    }
}

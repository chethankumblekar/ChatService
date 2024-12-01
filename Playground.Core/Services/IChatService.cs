﻿using PlayGround.ChatService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.ChatService.Core.Services
{
    public interface IChatService
    {
        Task SendMessageAync(Guid senderId,Guid recipientId, string message);
    }
}
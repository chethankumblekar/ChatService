using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Playground.ChatService.Core.Services;
using PlayGround.ChatService.DataService;
using PlayGround.ChatService.Models;

namespace PlayGround.ChatService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        [Authorize]
        public async Task SendMessageToUser(string recipientId, string message)
        {
            var senderId = Context.UserIdentifier!;
            await _chatService.SendMessageAync(senderId, recipientId, message);

            // notifying recipient
            await Clients.User(recipientId.ToString()).SendAsync("ReceiveMessage",senderId,message);
        }

    }
}

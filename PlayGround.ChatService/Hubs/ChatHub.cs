using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Playground.ChatService.Core.Services;
using PlayGround.ChatService.DataService;
using PlayGround.ChatService.Models;
using System.Collections.Concurrent;

namespace PlayGround.ChatService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        private static readonly ConcurrentDictionary<string, (string UserId, DateTime LastActive)> _onlineUsers = new();

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier!;
            _onlineUsers[Context.ConnectionId] =  (userId,DateTime.UtcNow);
            await Clients.All.SendAsync("UserConnected", userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _onlineUsers.TryRemove(Context.ConnectionId, out var userinfo);
            await Clients.All.SendAsync("UserDisconnected", userinfo.UserId);
            await base.OnDisconnectedAsync(exception);
        }

        [Authorize]
        public async Task SendMessageToUser(string recipientId, string message)
        {
            var senderId = Context.UserIdentifier!;
            if (_onlineUsers.ContainsKey(Context.ConnectionId))
            {
                _onlineUsers[Context.ConnectionId] = (senderId, DateTime.UtcNow);
            }

            await _chatService.SendMessageAync(senderId, recipientId, message);

            // notifying recipient
            await Clients.User(recipientId.ToString()).SendAsync("ReceiveMessage",senderId,message);
        }

    }
}

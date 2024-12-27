using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Playground.ChatService.Core.Services;
using PlayGround.ChatService.DataService;
using PlayGround.ChatService.Models;

namespace PlayGround.ChatService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SharedDb _sharedDb;
        
        //public ChatHub(SharedDb sharedDb) => _sharedDb = sharedDb;

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

        //[Authorize]
        //public async Task JoinChat(UserConnection userConnection)
        //{
        //    await Clients.All.SendAsync("RecievedMessage", "admin", $"{userConnection.UserName} has joined");
        //}

        //[Authorize]
        //public async Task JoinSpecificChatRoom(UserConnection userConnection)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ChatRoom);

        //    _sharedDb.connections[Context.ConnectionId] = userConnection;

        //    await Clients.Group(userConnection.ChatRoom)
        //        .SendAsync("RecieveSpecificMessage", "admin", $"{userConnection.UserName} has joined {userConnection.ChatRoom}");
        //}

        //[Authorize]
        //public async Task SendMessage(string message)
        //{
        //    if(_sharedDb.connections.TryGetValue(Context.ConnectionId,out UserConnection userConnection))
        //    {
        //        await Clients.Group(userConnection.ChatRoom)
        //            .SendAsync("RecieveSpecificMessage", userConnection.UserName, $"{message}");
        //    }
        //}

    }
}

using Playground.ChatService.Core.Services;
using PlayGround.ChatService.Core;
using PlayGround.ChatService.Model;

namespace PlayGround.ChatService.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IGroupRepository _groupRepository;
        public ChatService(
            IUserRepository userRepository, 
            IMessageRepository messageRepository, 
            IGroupRepository groupRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _groupRepository = groupRepository;
        }

        public async Task SendMessageAync(string senderId,string recipientId, string message)    {
            var sender = await _userRepository.GetUserByIdAsync(senderId);
            var recipient = await _userRepository.GetUserByIdAsync(recipientId);
            
            if(sender == null || recipient == null)
            {
                throw new Exception("User not found");
            }

            var userMessage = new Message()
            {
                Content = message,
                SenderId = senderId,
                RecipientId = recipientId,
            };
            
            await _messageRepository.AddMessageAsync(userMessage);

        }
    }
}

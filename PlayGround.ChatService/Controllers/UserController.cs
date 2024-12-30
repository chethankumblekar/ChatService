using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayGround.ChatService.Core;
using PlayGround.ChatService.Model;

namespace PlayGround.ChatService.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public UserController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpGet("messages")]
        public async Task<List<Message?>> GetLastMessages()
        {
            var currentUserId = User.Claims?.ToList()[0].Value;
            if (currentUserId != null) {
                
                return await _messageRepository.GetLastMessagesToOrFromUserAsync(currentUserId);
            }
            
            throw new Exception("user not logged in");
        }

        [HttpGet("messages/{recipientId}")]
        public async Task<List<Message>> GetMessages(string recipientId)
        {
            var currentUserId = User.Claims?.ToList()[0].Value;
            if (currentUserId != null)
            {
                return await _messageRepository.GetOneOnOneMessagesAsync(currentUserId,recipientId);
            }

            throw new Exception("user not logged in");
        }
    }
}

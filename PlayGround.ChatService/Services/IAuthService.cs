using PlayGround.ChatService.Model;
using PlayGround.ChatService.Models;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace PlayGround.ChatService.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(Payload payload);
    }
}

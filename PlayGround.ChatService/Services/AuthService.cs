using PlayGround.ChatService.Core;
using PlayGround.ChatService.Model;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace PlayGround.ChatService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AuthenticateAsync(Payload payload)
        {
            return await FindUserOrAddAsync(payload);
        }

        private async Task<User> FindUserOrAddAsync(Payload payload)
        {
            var u = await _userRepository.GetUserByIdAsync(payload.Email);
            if (u == null)
            {
                u = new User()
                {
                    Id = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    Email = payload.Email,
                };
                await _userRepository.AddUserAsync(u);
            }
            return u;
        }

    }
}

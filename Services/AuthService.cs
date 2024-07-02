using PlayGround.ChatService.Models;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace PlayGround.ChatService.Services
{
    public class AuthService : IAuthService
    {
        public AuthService()
        {
            Refresh();
        }

        private static IList<User> _users = new List<User>();
        public async Task<User> AuthenticateAsync(Payload payload)
        {
            await Task.Delay(1);
            return FindUserOrAdd(payload);
        }

        private User FindUserOrAdd(Payload payload)
        {
            var u = _users.FirstOrDefault(x => x.Email == payload.Email);
            if (u == null)
            {
                u = new User()
                {
                    Id = Guid.NewGuid(),
                    Name = payload.Name,
                    Email = payload.Email,
                    OAuthSubject = payload.Subject,
                    OAuthIssuer = payload.Issuer
                };
                _users.Add(u);
            }
            PrintUsers();
            return u;
        }

        private void PrintUsers()
        {
            string s = String.Empty;
            foreach (var u in _users) s += "\n[" + u.Email + "]";
            Console.WriteLine(s);
        }

        private void Refresh()
        {
            if (_users.Count == 0)
            {
                _users.Add(new User() { Id = Guid.NewGuid(), Name = "Test Person1", Email = "testperson1@gmail.com" });
                _users.Add(new User() { Id = Guid.NewGuid(), Name = "Test Person2", Email = "testperson2@gmail.com" });
                _users.Add(new User() { Id = Guid.NewGuid(), Name = "Test Person3", Email = "testperson3@gmail.com" });
                PrintUsers();
            }
        }
    }
}

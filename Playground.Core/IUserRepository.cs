using PlayGround.ChatService.Model;

namespace PlayGround.ChatService.Core
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User?> GetUserByIdAsync(string userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string userId);
    }
}
